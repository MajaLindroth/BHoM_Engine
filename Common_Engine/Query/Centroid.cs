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

using BH.Engine.Geometry;
using BH.oM.Common;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        public static Point Centroid(this IElement1D element1D)
        {
            //TODO: find a proper centre of weight of a curve (not an average of control points)
            throw new NotImplementedException();
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        public static Point Centroid(this IElement2D element2D)
        {
            Point tmp = Geometry.Query.Centroid(element2D.IOutlineCurve());
            double area = Geometry.Query.Area(element2D.IOutlineCurve());

            double x = tmp.X * area;
            double y = tmp.Y * area;
            double z = tmp.Z * area;


            List<PolyCurve> openings = Geometry.Compute.BooleanUnion(element2D.IInternalOutlineCurves());

            foreach (ICurve o in openings)
            {
                Point oTmp = Geometry.Query.ICentroid(o);
                double oArea = o.IArea();
                x -= oTmp.X * oArea;
                y -= oTmp.Y * oArea;
                z -= oTmp.Z * oArea;
                area -= oArea;
            }
            
            return new Point { X = x / area, Y = y / area, Z = z / area };
        }

        /******************************************/
    }
}
