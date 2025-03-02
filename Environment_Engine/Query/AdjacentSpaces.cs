/*
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a nested collection of Environment Panels which contain the search panel")]
        [Input("panel", "An Environment Panel to find within the search panels")]
        [Input("panelsAsSpaces", "The nested collection of Environment Panels that represent the spaces to search from")]
        [Output("panelsAsSpaces", "A nested collection of Environment Panels which contain the search panel")]
        public static List<List<Panel>> AdjacentSpaces(this Panel panel, List<List<Panel>> panelsAsSpaces)
        {
            //Get the lists which contain this building element
            return panelsAsSpaces.Where(x => x.Where(y => y.BHoM_Guid == panel.BHoM_Guid).Count() > 0).ToList();
        }
    }
}