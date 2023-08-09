using Intratech.Cores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intra.GeometryDetection
{
    public class AngleTypeParameters
    {
        public List<Vector2> pointsOnHull { get; set; }
        public List<Vector3> point3DsOnShape { get; set; }
        public List<Vector2> point2DsShapeOnPlane { get; set; }

        public AngleTypeParameters(List<Vector2> pointsOnHull, List<Vector3> point3DsOnShape, List<Vector2> point2DsShapeOnPlane)
        {
            this.pointsOnHull = pointsOnHull;
            this.point3DsOnShape = point3DsOnShape;
            this.point2DsShapeOnPlane = point2DsShapeOnPlane;
        }

        public void calculate(out Vector3? firstCenter, out Vector3? secondCenter, out double? maxDistance)
        {
            ListLines listLineItems = new ListLines(pointsOnHull);
            ListPoints listPoints = new ListPoints(point2Ds: pointsOnHull);

            List<double> angles = listLineItems.listAngleBetweenLines();
            Dictionary<Vector2, double> point2DAngleDictionary = listPoints.removedPointOnParallelLines(angles);
            List<double> listAngleDict = point2DAngleDictionary.Values.ToList();
            List<Vector2> point2DsRemoved = point2DAngleDictionary.Keys.ToList();

            Dictionary<Vector2, List<double>> sideLengthDictionary = new Dictionary<Vector2, List<double>>();

            for (int i = 0; i < listAngleDict.Count; i++)
            {
                if (isSquareAngle(listAngleDict[i]))
                {
                    if (i == 0)
                    {
                        var side1 = Vector2.Distance(point2DsRemoved[point2DsRemoved.Count - 1], point2DsRemoved[0]);
                        var side2 = Vector2.Distance(point2DsRemoved[1], point2DsRemoved[0]);
                        sideLengthDictionary.Add(point2DsRemoved[0], new List<double>() { side1, side2 });
                    }
                    else if (i == point2DsRemoved.Count - 1)
                    {
                        var side1 = Vector2.Distance(point2DsRemoved[point2DsRemoved.Count - 1], point2DsRemoved[0]);
                        var side2 = Vector2.Distance(point2DsRemoved[point2DsRemoved.Count - 2], point2DsRemoved[point2DsRemoved.Count - 1]);
                        sideLengthDictionary.Add(point2DsRemoved[point2DsRemoved.Count - 1], new List<double>() { side1, side2 });
                    }
                    else
                    {
                        var side1 = Vector2.Distance(point2DsRemoved[i - 1], point2DsRemoved[i]);
                        var side2 = Vector2.Distance(point2DsRemoved[i + 1], point2DsRemoved[i]);
                        sideLengthDictionary.Add(point2DsRemoved[i], new List<double>() { side1, side2 });
                    }
                }
            }

            Vector2 pointCenter = sideLengthDictionary.Select(x => x.Key)
                                                      .Where(x => sideLengthDictionary[x][0] >= sideLengthDictionary[x][1] * (1 - 0.2) && sideLengthDictionary[x][0] <= sideLengthDictionary[x][1] * (1 + 0.2))
                                                      .FirstOrDefault();

            List<float> listDistances = point2DsShapeOnPlane.Select(x => Vector2.Distance(x, pointCenter)).ToList();
            List<float> listDistancesAscending = listDistances.OrderBy(x => x).ToList();            
            float firstPoint = listDistancesAscending[0];
            float secondPoint = listDistancesAscending[1];

            LineItem lineCenter = new LineItem(pointFrom: point3DsOnShape[listDistances.IndexOf(firstPoint)], 
                                               pointTo: point3DsOnShape[listDistances.IndexOf(secondPoint)]);

            List<Vector3> projectionPoints = lineCenter.findProjectPointsOnLine(point3DsOnShape);

            listPoints.Point3Ds = projectionPoints;
            listPoints.twoPoint3DsWithMaxDistance(out firstCenter, out secondCenter);

            maxDistance = (firstCenter - secondCenter)?.Length;
        }

        private static bool isSquareAngle(double angle)
        {
            if (angle >= 90 * (1 - 0.2) && angle <= 90 * (1 + 0.2))
                return true;
            return false;
        }
    }
}
