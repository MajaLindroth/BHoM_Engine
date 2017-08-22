﻿using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetDistance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double GetSquareDistance(this Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double GetDistance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /***************************************************/

        public static double GetSquareDistance(this Vector a, Vector b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        /***************************************************/

        public static double GetDistance(this Point a, Plane plane)
        {
            Vector normal = plane.Normal.GetNormalised();
            return normal.GetDotProduct(a - plane.Origin);
        }

        /***************************************************/

        public static double GetDistance(this Point a, Line line)
        {
            return a.GetDistance(line._GetClosestPoint(a));
        }

        /***************************************************/

        public static double GetDistance(this Line line, Line other)
        {
            Point intersection = line.GetIntersection(other, false);
            if (intersection != null)
            {
                return 0;
            }
            else
            {
                List<double> distances = new List<double>();        //TODO: Can we do better than this?
                distances.Add(line.Start.GetDistance(other));
                distances.Add(line.End.GetDistance(other));
                distances.Add(other.Start.GetDistance(line));
                distances.Add(other.End.GetDistance(line));

                return distances.Min();
            }
        }
    }
}
