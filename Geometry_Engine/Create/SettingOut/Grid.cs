﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.SettingOut;

namespace BH.Engine.Geometry.SettingOut
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Grid Grid(ICurve curve)
        {
            return new Grid
            {
                Curve = Geometry.Modify.IProject(curve, BH.oM.Geometry.Plane.XY)
            };
        }

        /***************************************************/

        public static Grid Grid(Point origin, Vector direction, double length = 20)
        {
            Line line = new Line { Start = new Point { X = origin.X, Y = origin.Y, Z = 0 }, End = origin + new Vector { X = direction.X, Y = direction.Y, Z = 0 }.Normalise() * length };
            return new Grid { Curve = line };
        }

        /***************************************************/

        public static Grid Grid(ICurve curve, string name)
        {
            return new Grid
            {
                Curve = Geometry.Modify.IProject(curve, BH.oM.Geometry.Plane.XY),
                Name = name,
            };
        }

        /***************************************************/

        public static Grid Grid(Point origin, Vector direction, string name, double length = 20)
        {
            Line line = new Line { Start = new Point { X = origin.X, Y = origin.Y, Z = 0 }, End = origin + new Vector { X = direction.X, Y = direction.Y, Z = 0 }.Normalise() * length };
            return new Grid { Curve = line, Name = name };
        }

        /***************************************************/
    }
}