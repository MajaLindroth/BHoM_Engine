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

using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double ParameterAtPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            Point centre = curve.CoordinateSystem.Origin;
            Vector normal = curve.CoordinateSystem.Z;
            Vector v1 = curve.CoordinateSystem.X;
            Vector v2 = point - centre;

            double angle = v1.SignedAngle(v2, normal) - curve.StartAngle;
            angle = (Math.Abs(angle) < Tolerance.Angle) || Math.Abs(angle - 2 * Math.PI) < Tolerance.Angle ? 0 : angle;  //Really small negative angles gives wrong result. This solves that problem.
            return ((angle + 2 * Math.PI) % (2 * Math.PI)) / curve.Angle();
        }

        /***************************************************/

        public static double ParameterAtPoint(this Circle curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            Vector v1 = curve.StartPoint() - curve.Centre;
            Vector v2 = point - curve.Centre;
            return ((v1.SignedAngle(v2, curve.Normal) + 2 * Math.PI) % (2 * Math.PI)) / (2 * Math.PI);
        }

        /***************************************************/

        public static double ParameterAtPoint(this Line curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance)
                return -1;

            return point.Distance(curve.Start) / curve.Length();
        }

        /***************************************************/

        [NotImplemented]
        public static double ParameterAtPoint(this NurbsCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double ParameterAtPoint(this PolyCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            double length = 0;

            foreach (ICurve c in curve.SubParts())
            {
                if (c.IClosestPoint(point).SquareDistance(point) <= sqTol)
                    return (length + c.IParameterAtPoint(point, tolerance) * c.ILength()) / curve.ILength();
                else
                    length += c.ILength();
            }

            return -1;
        }

        /***************************************************/

        public static double ParameterAtPoint(this Polyline curve, Point point, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            double param = 0;

            foreach (Line l in curve.SubParts())
            {
                if (l.ClosestPoint(point).SquareDistance(point) <= sqTol)
                    return (param + l.ParameterAtPoint(point, tolerance) * l.Length()) / curve.Length();
                else
                    param += l.Length();
            }

            return -1;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IParameterAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            return ParameterAtPoint(curve as dynamic, point, tolerance);
        }

        /***************************************************/
    }
}