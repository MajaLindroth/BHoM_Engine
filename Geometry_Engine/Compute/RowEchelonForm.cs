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
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double[,] RowEchelonForm(this double[,] imatrix, bool reduced = true, double tolerance = Tolerance.Distance)
        {
            // Strongly inspired by https://rosettacode.org/wiki/Reduced_row_echelon_form

            double[,] matrix = (double[,])imatrix.Clone();
            int lead = 0, rowCount = matrix.GetLength(0), columnCount = matrix.GetLength(1);

            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount == lead)
                    break;

                int i = r;
                while (Math.Abs(matrix[i, lead]) < tolerance)
                {
                    i++;
                    if (i == rowCount)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead)
                        {
                            lead--;
                            break;
                        }
                    }
                }

                for (int j = 0; j < columnCount; j++)
                {
                    double temp = matrix[r, j];
                    matrix[r, j] = matrix[i, j];
                    matrix[i, j] = temp;
                }

                double div = matrix[r, lead];
                if (Math.Abs(div) >= tolerance)
                    for (int j = 0; j < columnCount; j++) matrix[r, j] /= div;

                int w = reduced ? 0 : r + 1;
                for (; w < rowCount; w++)
                {
                    if (w != r)
                    {
                        double sub = matrix[w, lead];
                        for (int k = 0; k < columnCount; k++)
                        {
                            matrix[w, k] -= (sub * matrix[r, k]);
                        }
                    }
                }

                lead++;
            }

            return matrix;
        }

        /***************************************************/

        public static int CountNonZeroRows(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            int c = 0;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Math.Abs(matrix[i, j]) >= tolerance)
                    {
                        c++;
                        break;
                    }
                }
            }

            return c;
        }

        /***************************************************/

        public static double REFTolerance(this double[,] matrix, double tolerance = Tolerance.Distance)
        {
            int d1 = matrix.GetLength(0);
            int d2 = matrix.GetLength(1);
            double maxRowSum = 0;

            for (int i = 0; i < d1; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < d2; j++)
                {
                    rowSum += Math.Abs(matrix[i, j]);
                }
                maxRowSum = Math.Max(maxRowSum, rowSum);
            }

            double result = tolerance * Math.Max(d1, d2) * maxRowSum;
            if (result >= 1)
                result = 1 - tolerance;

            return result;
        }

        /***************************************************/
    }
}
