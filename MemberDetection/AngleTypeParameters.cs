using Intratech.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace MemberDetection
{
    public class AngleTypeParameters
    {
        public List<Vector2> pointsOnHull {  get; set; }
        public PlaneItem planeItem { get; set; }
        public List<Vector3> point3DsOnShape { get; set; }
        public PlaneItem plane { get; set; }

        public AngleTypeParameters(List<Vector2> pointsOnHull, PlaneItem planeItem, List<Vector3> point3DsOnShape, PlaneItem plane)
        {
            this.pointsOnHull = pointsOnHull;
            this.planeItem = planeItem;
            this.point3DsOnShape = point3DsOnShape;
            this.plane = plane;
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
                        try
                        {
                            var side1 = Vector2.Distance(point2DsRemoved[i - 1], point2DsRemoved[i]);
                            var side2 = Vector2.Distance(point2DsRemoved[i + 1], point2DsRemoved[i]);
                            sideLengthDictionary.Add(point2DsRemoved[i], new List<double>() { side1, side2 });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"ERROR! The count of angles: {listAngleDict.Count} - i: {i}");
                        }
                    }
                }
            }

            Vector2 pointCenter = sideLengthDictionary.Select(x => x.Key)
                                                    .Where(x => sideLengthDictionary[x][0] >= sideLengthDictionary[x][1] * (1 - 0.2) && sideLengthDictionary[x][0] <= sideLengthDictionary[x][1] * (1 + 0.2))
                                                    .FirstOrDefault();

            listPoints.Point3Ds = point3DsOnShape;
            point3DsOnShape = listPoints.removeNearest3DPoints();
            var crossPoints = plane.get3DProjectionPointsOnPlane(point3DsOnShape);
            var point2DItems = plane.convert3DTo2D(crossPoints);

            List<float> listDistances = point2DItems.Select(x => Vector2.Distance(x, pointCenter)).ToList();
            List<int> indexes = Enumerable.Range(0, listDistances.Count - 1).Where(x => listDistances[x] >= -0.0001 && listDistances[x] <= 0.0001).ToList();

            LineItem lineCenter = new LineItem(point3DsOnShape[indexes[0]], point3DsOnShape[indexes[1]]);
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
