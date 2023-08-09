using Intratech.Cores;
using System.Collections.Generic;
using System.Linq;

namespace Intra.GeometryDetection
{
    class ListPoints
    {
        public List<Vector2> Point2Ds { get; set; } = new List<Vector2>();
        public List<Vector3> Point3Ds { get; set; } = new List<Vector3>();

        public ListPoints(List<Vector2> point2Ds)
        {
            Point2Ds = point2Ds;
        }

        public ListPoints(List<Vector3> point3Ds)
        {
            Point3Ds = point3Ds;
        }

        public List<Vector2> removeNearest2DPoints()
        {
            List<Vector2> newPointItems = new List<Vector2>();
            for (int i = 0; i < Point2Ds.Count; i++)
            {
                bool check = true;
                for (int j = i + 1; j < Point2Ds.Count; j++)
                {
                    if (Vector2.Distance(Point2Ds[i], Point2Ds[j]) < 0.0001)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    newPointItems.Add(Point2Ds[i]);
            }

            return newPointItems;
        }

        public List<Vector3> removeNearest3DPoints()
        {
            List<Vector3> newPointItems = new List<Vector3>();
            for (int i = 0; i < Point3Ds.Count; i++)
            {
                bool check = true;
                for (int j = i + 1; j < Point3Ds.Count; j++)
                {
                    if ((Point3Ds[i] - Point3Ds[j]).Length < 0.0001)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                    newPointItems.Add(Point3Ds[i]);
            }

            return newPointItems;
        }

        public Dictionary<Vector2, double> removedPointOnParallelLines(List<double> angles)
        {
            Dictionary<Vector2, double> point2DAngleDictionary = new Dictionary<Vector2, double>();        

            for (int i = 0; i < angles.Count; i++)
            {
                if (angles[i] >= 0.5 && angles[i] <= 179.5)
                {
                    point2DAngleDictionary.Add(Point2Ds[i], angles[i]);
                }
            }

            return point2DAngleDictionary;
        }

        public void twoPoint3DsWithMaxDistance(out Vector3? firstCenter, out Vector3? secondCenter)
        {
            Dictionary<List<Vector3>, double> distanceTwoPointDictionary = new Dictionary<List<Vector3>, double>();
            for (int i = 0; i < Point3Ds.Count - 1; i++)
            {
                for (int j = i + 1; j < Point3Ds.Count; j++)
                {
                    double distance = (Point3Ds[i] - Point3Ds[j]).Length;
                    List<Vector3> list = new List<Vector3>() { Point3Ds[i], Point3Ds[j] };
                    distanceTwoPointDictionary.Add(list, distance);
                }
            }

            var maxDistanceKey = distanceTwoPointDictionary.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            firstCenter = maxDistanceKey[0];
            secondCenter = maxDistanceKey[1];
        }
    }
}
