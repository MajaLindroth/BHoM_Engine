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

using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SectionProperties.Reinforcement;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConcreteSection SetReinforcement(this ConcreteSection section, IEnumerable<Reinforcement> reinforcement)
        {
            ConcreteSection clone = section.GetShallowClone() as ConcreteSection;
            clone.Reinforcement = reinforcement.ToList();
            return clone;
        }

        /***************************************************/

        public static ConcreteSection AddReinforcement(this ConcreteSection section, IEnumerable<Reinforcement> reinforcement)
        {
            ConcreteSection clone = section.GetShallowClone() as ConcreteSection;

            if (clone.Reinforcement == null)
                clone.Reinforcement = new List<Reinforcement>();
            else
                clone.Reinforcement = new List<Reinforcement>(clone.Reinforcement);


            clone.Reinforcement.AddRange(reinforcement);

            return clone;
        }

        /***************************************************/
    }
}
