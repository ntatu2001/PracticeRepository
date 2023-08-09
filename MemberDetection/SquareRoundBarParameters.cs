using Intratech.Cores;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace MemberDetection
{
    public class SquareRoundBarParameters
    {
        public List<Vector2> pointsOnHull { get; set; }
        public PlaneItem planeItem { get; set; }
        public List<Vector3> point3DsOnShape { get; set; }

        public SquareRoundBarParameters(List<Vector2> pointsOnHull, PlaneItem planeItem, List<Vector3> point3DsOnShape)
        {
            this.pointsOnHull = pointsOnHull;
            this.planeItem = planeItem;
            this.point3DsOnShape = point3DsOnShape;
        }

        public void calculate(out Vector3? firstCenter, out Vector3? secondCenter)
        {
            float x_center = 0;
            float y_center = 0;
            foreach (Vector2 pointItem in pointsOnHull)
            {
                x_center += pointItem.x;
                y_center += pointItem.y;
            }

            Vector2 pointCenter = new Vector2(x: x_center / pointsOnHull.Count,
                                              y: y_center / pointsOnHull.Count);

            List<Vector2> pointItems = new List<Vector2>() { pointCenter };
            List<Vector3> point3DOnHull = planeItem.convert2DTo3D(pointItems);

            LineItem lineCenter = new LineItem(vector: planeItem.normalVector,
                                               pointFrom: point3DOnHull[0],
                                               pointTo: null);

            List<Vector3> projectionPoints = lineCenter.findProjectPointsOnLine(point3DsOnShape);

            ListPoints listPoints = new ListPoints(point3Ds: projectionPoints);
            listPoints.twoPoint3DsWithMaxDistance(out firstCenter, out secondCenter);
        }
    }
}
