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

using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Panel with the provided space name added as a connected space")]
        [Input("panels", "A collection of Environment Panels to add the space name to")]
        [Input("spaceName", "The name of the space the panels are connected to")]
        [Output("panelsAsSpace", "A collection of modified Environment Panels with the provided space name listed as a connecting space")]
        public static List<Panel> AddAdjacentSpace(this List<Panel> panels, string spaceName)
        {
            foreach(Panel p in panels)
            {
                if(p.ConnectedSpaces.Count < 2)
                    p.ConnectedSpaces.Add(spaceName);
                else
                    p.ConnectedSpaces[1] = spaceName;
            }

            return panels;
        }

        [Description("Returns a single Environment Panel with the provided space name added as a connected space")]
        [Input("panel", "A single Environment Panel to add the space name to")]
        [Input("spaceName", "The name of the space the panel is connected to")]
        [Output("panel", "A modified Environment Panel with the provided space name listed as a connecting space")]
        public static Panel AddAdjacentSpace(this Panel panel, string spaceName)
        {
            return AddAdjacentSpace(new List<Panel> { panel }, spaceName)[0];
        }

        [Description("Returns a single Environment Panel with the provided space names added as the connecting spaces")]
        [Input("panel", "A single Environment Panel to add the space names to")]
        [Input("spaceNames", "The collection of names of the spaces the panel is connected to")]
        [Output("panel", "A modified Environment Panel with the provided space names listed as the connecting spaces")]
        public static Panel SetAdjancentSpaces(this Panel panel, List<string> spaceNames)
        {
            panel.ConnectedSpaces = spaceNames;
            return panel;
        }

        [Description("Returns a single Environment Panel with an updated connected space name")]
        [Input("panel", "A single Environment Panel to change the connected space name of")]
        [Input("spaceNameToChange", "The space name to replace")]
        [Input("replacementSpaceName", "The new space name to use")]
        [Output("panel", "A modified Environment Panel with the changed connected space name")]
        public static Panel ChangeAdjacentSpace(this Panel panel, string spaceNameToChange, string replacementSpaceName)
        {
            for(int x = 0; x < panel.ConnectedSpaces.Count; x++)
            {
                if (panel.ConnectedSpaces[x] == spaceNameToChange)
                    panel.ConnectedSpaces[x] = replacementSpaceName;
            }

            return panel;
        }

        [Description("Returns a collection of Environment Panels where any connected spaces which are detailed within the spaceNamesToChange are replaced by a replacementSpaceName. The spaceNamesToChange and replacementSpaceNames should match length to provide a 1:1 change")]
        [Input("panels", "A collection of Environment Panels to update the connected space names of")]
        [Input("spaceNamesToChange", "A collection of space names which should be updated")]
        [Input("replacementSpaceNames", "A collection of space names to replace with")]
        [Output("panels", "A collection of Environment Panels modified so that space names are changed as appropriate")]
        public static List<Panel> ChangeAdjacentSpaces(this List<Panel> panels, List<string> spaceNamesToChange, List<string> replacementSpaceNames)
        {
            if(spaceNamesToChange.Count != replacementSpaceNames.Count)
            {
                BH.Engine.Reflection.Compute.RecordError("Please ensure the number of replacement space names matches the number of changing space names. Panels returned without change");
                return panels;
            }

            for(int x = 0; x < spaceNamesToChange.Count; x++)
            {
                for (int a = 0; a < panels.Count; a++)
                    panels[a] = ChangeAdjacentSpace(panels[a], spaceNamesToChange[x], replacementSpaceNames[x]);
            }

            return panels;
        }
    }
}
