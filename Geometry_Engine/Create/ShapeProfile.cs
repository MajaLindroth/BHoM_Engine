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

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProfile ISectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> curves = IProfileCurves(flangeThickness, width, flangeThickness, width, webthickness, height - 2 * flangeThickness, rootRadius, toeRadius);
            return new ISectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

        public static BoxProfile BoxProfile(double height, double width, double thickness, double outerRadius, double innerRadius)
        {
            List<ICurve> curves = BoxProfileCurves(width, height, thickness, thickness, innerRadius, outerRadius);
            return new BoxProfile(height, width, thickness, outerRadius, innerRadius, curves);
        }

        /***************************************************/

        public static AngleProfile AngleProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ = false, bool mirrorAboutLocalY = false)
        {
            List<ICurve> curves = AngleProfileCurves(width, height, flangeThickness, webthickness, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();
            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new AngleProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, mirrorAboutLocalY, curves);
        }

        /***************************************************/

        public static ChannelProfile ChannelProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ = false)
        {
            List<ICurve> curves = ChannelProfileCurves(height, width, webthickness, flangeThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();

            return new ChannelProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/

        public static CircleProfile CircleProfile(double diameter)
        {
            List<ICurve> curves = CircleProfileCurves(diameter / 2);
            return new CircleProfile(diameter, curves);
        }

        /***************************************************/

        public static FabricatedBoxProfile FabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            List<ICurve> curves = FabricatedBoxProfileCurves(width, height, webThickness, topFlangeThickness, botFlangeThickness);
            return new FabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/

        public static GeneralisedFabricatedBoxProfile GeneralisedFabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness = 0.0, double botFlangeThickness = 0.0, double topCorbelWidth = 0.0, double botCorbelWidth = 0.0)
        {
            List<ICurve> curves = GeneralisedFabricatedBoxProfileCurves(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth);
            return new GeneralisedFabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth, curves);
        }

        /***************************************************/

        public static KiteProfile KiteProfile(double width1, double angle1, double thickness)
        {
            List<ICurve> curves = KiteProfileCurves(width1, angle1, thickness);
            return new KiteProfile(width1, angle1, thickness, curves);
        }

        /***************************************************/

        public static FabricatedISectionProfile FabricatedISectionProfile(double height, double topFlangeWidth, double botFlangeWidth, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            List<ICurve> curves = IProfileCurves(topFlangeThickness, topFlangeWidth, botFlangeThickness, botFlangeWidth, webThickness, height - botFlangeThickness - topFlangeThickness,0,0);
            return new FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/

        public static FreeFormProfile FreeFormProfile(IEnumerable<ICurve> edges)
        {
            return new FreeFormProfile(edges);
        }

        /***************************************************/

        public static RectangleProfile RectangleProfile(double height, double width, double cornerRadius)
        {
            List<ICurve> curves = RectangleProfileCurves(width, height, cornerRadius);
            return new RectangleProfile(height, width, cornerRadius, curves);
        }

        /***************************************************/

        public static TSectionProfile TSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalY = false)
        {
            List<ICurve> curves = TeeProfileCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new TSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalY, curves);
        }

        /***************************************************/

        public static GeneralisedTSectionProfile GeneralisedTSectionProfile(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness, bool mirrorAboutLocalY = false)
        {
            List<ICurve> curves = GeneralisedTeeProfileCurves(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness);

            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new GeneralisedTSectionProfile(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness,mirrorAboutLocalY, curves);
        }

        /***************************************************/

        public static TubeProfile TubeProfile(double diameter, double thickness)
        {
            List<ICurve> curves = TubeProfileCurves(diameter / 2, thickness);
            return new TubeProfile(diameter, thickness, curves);
        }

        /***************************************************/

        [NotImplemented]
        public static ZSectionProfile ZSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            throw new NotImplementedException();
            //TODO: Section curves for z-profile
            //List<ICurve> curves = ZProfileCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);
            //return new ZSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> MirrorAboutLocalY(this List<ICurve> curves)
        {
            Plane plane = oM.Geometry.Plane.XZ;
            return curves.Select(x => x.IMirror(plane)).ToList();
        }

        /***************************************************/

        private static List<ICurve> MirrorAboutLocalZ(this List<ICurve> curves)
        {
            Plane plane = oM.Geometry.Plane.YZ;
            return curves.Select(x => x.IMirror(plane)).ToList();
        }

        /***************************************************/
    }
}
