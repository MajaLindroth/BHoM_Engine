/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - ConcreteSEction          ****/
        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this ConcreteSection property, double xStart = 0, double xEnd = 1)
        {
            CompositeGeometry geometry = new CompositeGeometry();
            foreach (Reinforcement reo in property.Reinforcement)
            {
                CompositeGeometry layout = reo.ILayout(property);

                foreach (IGeometry obj in layout.Elements)
                {
                    if (obj is Point)
                    {
                        geometry.Elements.Add(new Circle { Centre = obj as Point, Normal = Vector.ZAxis, Radius = reo.Diameter / 2 });
                    }
                    else
                    {
                        geometry.Elements.Add(obj);
                    }
                }
            }


            return geometry;
        }


        /***************************************************/
        /**** Public Methods - Reinforcement            ****/
        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this LayerReinforcement reinforcement, ConcreteSection property, bool extrude = false)
        {
            BoundingBox bounds = new BoundingBox();

            foreach (ICurve curve in property.SectionProfile.Edges)
            {
                bounds += curve.IBounds();
            }

            double relativeDepth = reinforcement.IsVertical ? bounds.Max.X - reinforcement.Depth : bounds.Max.Y - reinforcement.Depth;
            double[] range = null;
            double tieDiameter = property.TieDiameter();
            if (property.SectionProfile.Shape == ShapeType.Rectangle && tieDiameter > 0)
            {
                //TODO: Check this part
                tieDiameter = tieDiameter + Math.Cos(Math.PI / 4) * (2 * tieDiameter * (Math.Sqrt(2) - 1) + reinforcement.Diameter / 2) - reinforcement.Diameter / 2;
            }
            double width = reinforcement.IsVertical ? property.SectionProfile.DepthAt(relativeDepth, ref range) : property.SectionProfile.WidthAt(relativeDepth, ref range);

            double spacing = (width - 2 * property.MinimumCover - reinforcement.Diameter - 2 * tieDiameter) / (reinforcement.BarCount - 1.0);
            double start = range != null && range.Length > 0 ? range[0] : 0;

            List<Point> location = new List<Point>();

            for (int i = 0; i < reinforcement.BarCount; i++)
            {
                double x = reinforcement.IsVertical ? relativeDepth : property.MinimumCover + reinforcement.Diameter / 2 + tieDiameter + spacing * i + start;
                double y = reinforcement.IsVertical ? property.MinimumCover + reinforcement.Diameter / 2 + spacing * i + tieDiameter + start : relativeDepth;

                location.Add(new Point { X = x, Y = y, Z = 0 });
            }

            return new CompositeGeometry { Elements = location.ToList<IGeometry>() };


            //GeometryGroup<Point> location = new GeometryGroup<Point>();
            //for (int i = 0; i < BarCount; i++)
            //{
            //    double x = IsVertical ? relativeDepth : property.MinimumCover + Diameter / 2 + tieDiameter + spacing * i + start;
            //    double y = IsVertical ? property.MinimumCover + Diameter / 2 + spacing * i + tieDiameter + start : relativeDepth;

            //    location.Add(new Point { X = x, Y = y, Z = 0 });
            //}
            //return location;
        }

        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this PerimeterReinforcement reinforcement, ConcreteSection property, bool extrude = false)
        {
            return Layout(reinforcement, property.SectionProfile as dynamic, property);
        }

        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this PerimeterReinforcement reinforcement, RectangleProfile dimensions, ConcreteSection property)
        {
            double h = dimensions.Height;
            double w = dimensions.Width;
            double tieDiameter = property.TieDiameter();
            List<Point> location = new List<Point>();

            int topCount = 0;
            int sideCount = 0;
            double tieOffset = tieDiameter + Math.Cos(Math.PI / 4) * (2 * tieDiameter * (Math.Sqrt(2) - 1) + reinforcement.Diameter / 2) - reinforcement.Diameter / 2;
            switch (reinforcement.Pattern)
            {
                case ReinforcementPattern.Equispaced:
                    topCount = (int)(reinforcement.BarCount * w / (2 * w + 2 * h) + 1);
                    sideCount = (reinforcement.BarCount - 2 * topCount) / 2 + 2;
                    break;
                case ReinforcementPattern.Horizontal:
                    topCount = reinforcement.BarCount / 2;
                    sideCount = 2;
                    break;
                case ReinforcementPattern.Vertical:
                    topCount = 2;
                    sideCount = reinforcement.BarCount / 2;
                    break;
            }
            double verticalSpacing = (h - 2 * property.MinimumCover - reinforcement.Diameter - 2 * tieOffset) / (sideCount - 1);
            double depth = property.MinimumCover + reinforcement.Diameter / 2 + tieOffset;
            for (int i = 0; i < sideCount; i++)
            {
                int count = topCount;
                double currentDepth = depth + i * verticalSpacing;
                if (i > 0 && i < sideCount - 1)
                {
                    count = 2;
                }
                List<IGeometry> layout = ((CompositeGeometry)new LayerReinforcement { Diameter = reinforcement.Diameter, Depth = currentDepth, BarCount = count }.Layout(property)).Elements;

                foreach (IGeometry geom in layout)
                {
                    location.Add(geom as Point);
                }
            }
            return new CompositeGeometry { Elements = location.ToList<IGeometry>() };
        }

        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this PerimeterReinforcement reinforcement, CircleProfile dimensions, ConcreteSection property)
        {
            double d = dimensions.Diameter;
            List<Point> location = new List<Point>();

            double angle = Math.PI * 2 / reinforcement.BarCount;
            double startAngle = 0;
            double radius = d / 2 - property.MinimumCover - reinforcement.Diameter / 2;
            switch (reinforcement.Pattern)
            {
                case ReinforcementPattern.Horizontal:
                    startAngle = angle / 2;
                    break;
            }
            for (int i = 0; i < reinforcement.BarCount; i++)
            {
                double x = Math.Cos(startAngle + angle * i) * radius;
                double y = Math.Sin(startAngle + angle * i) * radius;
                location.Add(new Point { X = x, Y = y, Z = 0 });
            }


            return new CompositeGeometry { Elements = location.ToList<IGeometry>() };
        }

        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this PerimeterReinforcement reinforcement, IProfile dimensions, ConcreteSection property)
        {

            //TODO: Implement for various cross section types
            return new CompositeGeometry();
        }

        /***************************************************/

        //public static CompositeGeometry Layout(this PerimeterReinforcement reinforcement, ConcreteSection property)
        //{
        //    double d = property.TotalDepth;
        //    double w = property.TotalWidth;
        //    double tieDiameter = property.TieDiameter();
        //    List<Point> location = new List<Point>();
        //    if (property.Shape == ShapeType.Rectangle) //Rectangle
        //    {
        //        int topCount = 0;
        //        int sideCount = 0;
        //        double tieOffset = tieDiameter + Math.Cos(Math.PI / 4) * (2 * tieDiameter * (Math.Sqrt(2) - 1) + reinforcement.Diameter / 2) - reinforcement.Diameter / 2;
        //        switch (reinforcement.Pattern)
        //        {
        //            case ReinforcementPattern.Equispaced:
        //                topCount = (int)(reinforcement.BarCount * w / (2 * w + 2 * d) + 1);
        //                sideCount = (reinforcement.BarCount - 2 * topCount) / 2 + 2;
        //                break;
        //            case ReinforcementPattern.Horizontal:
        //                topCount = reinforcement.BarCount / 2;
        //                sideCount = 2;
        //                break;
        //            case ReinforcementPattern.Vertical:
        //                topCount = 2;
        //                sideCount = reinforcement.BarCount / 2;
        //                break;
        //        }
        //        double verticalSpacing = (d - 2 * property.MinimumCover - reinforcement.Diameter - 2 * tieOffset) / (sideCount - 1);
        //        double depth = property.MinimumCover + reinforcement.Diameter / 2 + tieOffset;
        //        for (int i = 0; i < sideCount; i++)
        //        {
        //            int count = topCount;
        //            double currentDepth = depth + i * verticalSpacing;
        //            if (i > 0 && i < sideCount - 1)
        //            {
        //                count = 2;
        //            }
        //            List<IGeometry> layout = ((CompositeGeometry)new LayerReinforcement(reinforcement.Diameter, currentDepth, count).Layout(property)).Elements;

        //            foreach (IGeometry geom in layout)
        //            {
        //                location.Add(geom as Point);
        //            }
        //        }
        //    }
        //    else if (property.Shape == ShapeType.Circle) //Circular
        //    {
        //        double angle = Math.PI * 2 / reinforcement.BarCount;
        //        double startAngle = 0;
        //        double radius = d / 2 - property.MinimumCover - reinforcement.Diameter / 2;
        //        switch (reinforcement.Pattern)
        //        {
        //            case ReinforcementPattern.Horizontal:
        //                startAngle = angle / 2;
        //                break;
        //        }
        //        for (int i = 0; i < reinforcement.BarCount; i++)
        //        {
        //            double x = Math.Cos(startAngle + angle * i) * radius;
        //            double y = Math.Sin(startAngle + angle * i) * radius;
        //            location.Add(new Point { X = x, Y = y, Z = 0 });
        //        }

        //    }
        //    return new CompositeGeometry(location);
        //}

        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry Layout(this TieReinforcement reinforcement, ConcreteSection property, bool extrude = false)
        {
            return new CompositeGeometry();
            //double tieDiameter = property.TieDiameter();
            //switch (property.Shape)
            //{
            //    case ShapeType.Rectangle:
            //        double X = property.TotalWidth / 2 - property.MinimumCover - tieDiameter * 3;
            //        double Y = property.TotalDepth / 2 - property.MinimumCover - tieDiameter * 3;
            //        double yIn = property.TotalDepth / 2 - property.MinimumCover - tieDiameter / 2;
            //        double xIn = property.TotalWidth / 2 - property.MinimumCover - tieDiameter / 2;

            //        //TODO: Implement. Below copied from BHoM 1.0


            //        /*TEMP****************
            //        Group<Curve> curves = new Group<Curve>();
            //        curves.Add(new Line(new Point { X = -X, Y = yIn, Z = 0 }, new Point { X = X, Y = yIn, Z = 0 }));
            //        curves.Add(new Line(new Point { X = -X, Y = -yIn, Z = 0 }, new Point { X = X, Y = -yIn, Z = 0 }));
            //        curves.Add(new Line(new Point { X = xIn, Y = -Y, Z = 0 }, new Point { X = xIn, Y = Y, Z = -tieDiameter }));
            //        curves.Add(new Line(new Point { X = -xIn, Y = -Y, Z = 0 }, new Point { X = -xIn, Y = Y, Z = 0 }));
            //        Plane p = new Plane(new Point { X = -X, Y = -Y, Z = 0 }, Vector.ZAxis());
            //        curves.Add(new Arc(Math.PI * 3 / 2, Math.PI, tieDiameter * 2.5, p));
            //        p = new Plane(new Point { X = -X, Y = Y, Z = 0 }, Vector.ZAxis());
            //        curves.Add(new Arc(Math.PI, Math.PI / 2, tieDiameter * 2.5, p));
            //        p = new Plane(new Point { X = X, Y = Y, Z = 0 }, Vector.ZAxis());
            //        Vector lap = new Vector { X = -tieDiameter * 3.5, Y = -tieDiameter * 3.5, Z = 0 };
            //        Arc a1 = new Arc(Math.PI / 2, -Math.PI / 4, tieDiameter * 2.5, p);
            //        curves.Add(a1);
            //        curves.Add(new Line(a1.EndPoint, a1.EndPoint + lap));
            //        p = new Plane(new Point { X = X, Y = Y, Z = -tieDiameter }, Vector.ZAxis());
            //        Arc a2 = new Arc(0, 3 * Math.PI / 4, tieDiameter * 2.5, p);
            //        curves.Add(a2);
            //        curves.Add(new Line(a2.EndPoint, a2.EndPoint + lap));
            //        p = new Plane(new Point { X = X, Y = -Y, Z = 0 }, Vector.ZAxis());
            //        curves.Add(new Arc(0, -Math.PI / 2, tieDiameter * 2.5, p));

            //        Curve c = Curve.Join(curves)[0];

            //        double width = property.TotalWidth - 2 * property.MinimumCover - tieDiameter;
            //        double spacing = width / (BarCount - 1);
            //        Curve singleTie = null;
            //        if (BarCount > 2)
            //        {
            //            List<Curve> crvs = new List<Curve>();
            //            double startAngle = 0;
            //            double endAngle = Math.PI * 3 / 4;                       
            //            Vector lap2 = lap.DuplicateVector();
            //            p = new Plane(new Point { X = 0, Y = property.TotalDepth / 2 - property.MinimumCover - 3 * tieDiameter, Z = -tieDiameter }, Vector.ZAxis());
            //            a1 = new Arc(startAngle, endAngle, 2.5 * tieDiameter, p);
            //            a2 = a1.DuplicateCurve() as Arc;
            //            a2.Mirror(Plane.XZ());
            //            lap2.Mirror(Plane.XZ());
            //            crvs.Add(new Line(a1.StartPoint, a2.StartPoint));
            //            crvs.Add(new Line(a1.EndPoint, a1.EndPoint + lap));
            //            crvs.Add(new Line(a2.EndPoint, a2.EndPoint + lap2));
            //            crvs.Add(a1);
            //            crvs.Add(a2);
            //            singleTie = Curve.Join(crvs)[0];
            //        }

            //        Group<Pipe> bars = new Group<Pipe>();
            //        bars.Add(new Pipe(c, tieDiameter / 2));
            //        for (int i = 0; i < BarCount - 2; i++)
            //        {
            //            c = singleTie.DuplicateCurve();
            //            double location = -width / 2 + (i + 1) * spacing;
            //            //if (location < 0)
            //            //{
            //            //    c.Mirror(Plane.YZ());
            //            //}
            //            //TEMP UNDO c.Translate(Vector.XAxis(location));
            //            bars.Add(new Pipe(c, tieDiameter / 2));
            //        }
            //        */
            //        return null;//temp bars;

            //        //double X = property.TotalWidth / 2 - property.MinimumCover - tieDiameter * 3;
            //        //double Y = property.TotalDepth / 2 - property.MinimumCover - tieDiameter * 3;
            //        //double yIn = property.TotalDepth / 2 - property.MinimumCover - tieDiameter;
            //        //double yOut = property.TotalDepth / 2 - property.MinimumCover;
            //        //double xIn = property.TotalWidth / 2 - property.MinimumCover - tieDiameter;
            //        //double xOut = property.TotalWidth / 2 - property.MinimumCover;

            //        //Group<Curve> curves = new Group<Curve>();
            //        //curves.Add(new Line(new Point { X = -X, Y = yIn, Z = 0 }, new Point { X = X, Y = yIn, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = -X, Y = -yIn, Z = 0 }, new Point { X = X, Y = -yIn, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = -X, Y = yOut, Z = 0 }, new Point { X = X, Y = yOut, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = -X, Y = -yOut, Z = 0 }, new Point { X = X, Y = -yOut, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = xIn, Y = -Y, Z = 0 }, new Point { X = xIn, Y = Y, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = -xIn, Y = -Y, Z = 0 }, new Point { X = -xIn, Y = Y, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = xOut, Y = -Y, Z = 0 }, new Point { X = xOut, Y = Y, Z = 0 }));
            //        //curves.Add(new Line(new Point { X = -xOut, Y = -Y, Z = 0 }, new Point { X = -xOut, Y = Y, Z = 0 }));
            //        //Plane p = new Plane(new Point { X = -X, Y = -Y, Z = 0 }, Vector.ZAxis());
            //        //curves.Add(new Arc(Math.PI * 3 / 2, Math.PI, tieDiameter * 2, p));
            //        //curves.Add(new Arc(Math.PI * 3 / 2, Math.PI, tieDiameter * 3, p));
            //        //p = new Plane(new Point { X = -X, Y = Y, Z = 0 }, Vector.ZAxis());
            //        //curves.Add(new Arc(Math.PI, Math.PI / 2, tieDiameter * 2, p));
            //        //curves.Add(new Arc(Math.PI, Math.PI / 2, tieDiameter * 3, p));
            //        //p = new Plane(new Point { X = X, Y = Y, Z = 0 }, Vector.ZAxis());
            //        //curves.Add(new Arc(Math.PI / 2, 0, tieDiameter * 2, p));
            //        //curves.Add(new Arc(Math.PI / 2, 0, tieDiameter * 3, p));
            //        //p = new Plane(new Point { X = X, Y = -Y, Z = 0 }, Vector.ZAxis());
            //        //curves.Add(new Arc(0, -Math.PI / 2, tieDiameter * 2, p));
            //        //curves.Add(new Arc(0, -Math.PI / 2, tieDiameter * 3, p));
            //        //return new Group<Curve>(Curve.Join(curves));
            //}

            //return null;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [NotImplemented]
        public static CompositeGeometry ILayout(this Reinforcement reinforcement, ConcreteSection property, bool extrude = false)
        {
            return Layout(reinforcement as dynamic, property, extrude);
        }

        /***************************************************/
    }
}
