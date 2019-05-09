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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether the space is closed by ensuring all edges are connected to other elements")]
        [Input("panelsAsSpace", "The collection of Environment Panels that represent the space to check")]
        [Output("unboundEdges", "The unbound edges of the space")]
        public static List<Line> UnboundEdges(this List<Panel> panelsAsSpace)
        {
            //Check that each edge is connected to at least one other edge
            List<Line> edgeParts = new List<Line>();
            foreach (Panel p in panelsAsSpace)
                edgeParts.AddRange(p.ToLines());

            List<Line> unique = edgeParts.Distinct().ToList();

            List<Line> unboundEdges = new List<Line>();

            foreach (Line l in unique)
            {
                if (edgeParts.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                    unboundEdges.Add(l);
            }

            return unboundEdges;
        }
    }
}
