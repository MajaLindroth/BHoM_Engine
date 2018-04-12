﻿using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Direction(this Line line, double tolerance = Tolerance.Distance)
        {
            if (line.Start.SquareDistance(line.End) <= tolerance * tolerance) return new Vector { X = 0, Y = 0, Z = 0 };
            return new Vector { X = line.End.X - line.Start.X, Y = line.End.Y - line.Start.Y, Z = line.End.Z - line.Start.Z }.Normalise();
        }

        /***************************************************/

        public static Vector Normal(this Mesh mesh, Face face)
        {
            List<Point> vertices = mesh.Vertices;

            Point p1 = vertices[(face.A)];
            Point p2 = vertices[(face.B)];
            Point p3 = vertices[(face.C)];
            return Query.CrossProduct(p2 - p1, p3 - p1);
        }

        /***************************************************/
        public static Vector TangentAt(this NurbCurve curve, double t)
        {

            Vector sumNwP = new Vector { X = 0, Y = 0, Z = 0 };
            Vector sumNwPDer = new Vector { X = 0, Y = 0, Z = 0 };
            double sumNw = 0;
            double sumNwDer = 0;

            int degree = curve.Degree();

            for (int i = 0; i < curve.ControlPoints.Count; i++)
            {
                double Nt = curve.BasisFunction(i, degree, t);
                double Nder = curve.DerivativeFunction(i, degree, t);
                Vector p = Create.Vector(curve.ControlPoints[i]);
                sumNwP += p * Nt * curve.Weights[i];
                sumNwPDer += p * Nder * curve.Weights[i];
                sumNw += Nt * curve.Weights[i];
                sumNwDer += Nder * curve.Weights[i];
            }
            Vector tangent = sumNwPDer * sumNw - sumNwP * sumNwDer;
            return tangent.Normalise();

        }


    }
}
