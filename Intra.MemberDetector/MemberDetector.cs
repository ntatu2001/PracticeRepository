using Intratech.Cores;
using Intratech.Cores.Common;
using Intratech.Cores.Geometries;
using Intratech.Cores.Geometries.Line;
using Intratech.Cores.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Intra.MemberDetector
{
    public enum MemberPartType
    {
        None,
        Angle,
        SquareBar,
        RoundBar
    }

    public class MemberDetector
    {
        public static float SameAngleDegreeTolerance = 1f;//0.5 degree
        public static float SameAngleRadianTolerance = 0.001f;//0.001 Radian
        public static float SamePointDistanceTolerance = 0.002f;//2mm
        //public static float ToleranceForDirectionLine = 0.0000001F;
        public static float ToleranceForDirectionLine = 0.001F;


        public Mesh Mesh;
        public Vector3 _direction;//including length
        public Vector3 _start;
        public Vector3 _end;
        public MemberDetector(Mesh mesh)
        {
            Mesh = mesh;
            if(DetectStartEndPoint(in mesh ,out _end, out _start))
            {
                _direction = (_end - _start);
                //LogUtility.Instance.Log($"Direction: ({_direction.x}, {_direction.y}, {_direction.z})", LogType.Debug);
            }
        }

        private bool DetectStartEndPoint(in Mesh mesh, out Vector3 end, out Vector3 start)
        {
            //TODO:
            end = start = default;
            var lines = createLinesFromTriangles(mesh);
            var directionLine = getDirectionLineGroup(lines);

            start = directionLine.pointFrom;
            end = directionLine.pointTo;
            return end != default && start != default;
        }

        private List<Line> createLinesFromTriangles(Mesh mesh)
        {
            List<Vector3> Vertices = mesh.Points;
            List<int> Faces = mesh.Faces;
            Dictionary<int, List<int>> lineDictionary = Enumerable.Range(0, Vertices.Count).ToDictionary(x => x, x => new List<int>());

            List<Line> lines = new List<Line>();
            for (int i = 0; i < Faces.Count; i++)
            {
                if (i % 3 == 0)
                {
                    if (!lineDictionary[Faces[i]].Contains(Faces[i + 1]))
                    {
                        lines.Add(new Line(Vertices[Faces[i]], Vertices[Faces[i + 1]]));
                        lineDictionary[Faces[i]].Add(Faces[i + 1]);
                        lineDictionary[Faces[i + 1]].Add(Faces[i]);
                    }

                    if (!lineDictionary[Faces[i]].Contains(Faces[i + 2]))
                    {
                        lines.Add(new Line(Vertices[Faces[i]], Vertices[Faces[i + 2]]));
                        lineDictionary[Faces[i]].Add(Faces[i + 2]);
                        lineDictionary[Faces[i + 2]].Add(Faces[i]);
                    }

                    if (!lineDictionary[Faces[i + 1]].Contains(Faces[i + 2]))
                    {
                        lines.Add(new Line(Vertices[Faces[i + 1]], Vertices[Faces[i + 2]]));
                        lineDictionary[Faces[i + 1]].Add(Faces[i + 2]);
                        lineDictionary[Faces[i + 2]].Add(Faces[i + 1]);
                    }
                }
            }

            return lines;
        }

        private Line getDirectionLineGroup(List<Line> lineItems)
        {
            //Given group of parallel lines
            Dictionary<Line, List<Line>> parallelLineDictionary = new Dictionary<Line, List<Line>> { { lineItems[0], new List<Line>() { lineItems[0] } } };
            for (int i = 1; i < lineItems.Count; i++)
            {
                bool isParallel = false;
                foreach (Line lineKey in parallelLineDictionary.Keys)
                {
                    if (Vector3.Parallel(lineKey.vector, lineItems[i].vector, ToleranceForDirectionLine))
                    {
                        isParallel = true;
                        parallelLineDictionary[lineKey].Add(lineItems[i]);
                    }
                }

                if (!isParallel)
                    parallelLineDictionary.Add(lineItems[i], new List<Line>() { lineItems[i] });
            }

            //Filter groups which have the number of lines higher than 3, create a new list of List lines sorted by the count.
            List<List<Line>> group = parallelLineDictionary.Where(x => x.Value.Count > 3).Select(y => y.Value).OrderByDescending(x => x.Count).ToList();
            //LogUtility.Instance.Log($"The count of each group:", LogType.Debug);
            //foreach (List<Line> lines in group)
            //{
            //    LogUtility.Instance.Log($"group {group.IndexOf(lines)} has {lines.Count} lines", LogType.Debug);
            //}

            //Create a new List of list of each vector's length in each group.
            List<List<float>> lengthOfEachGroup = group.Select(x => x.Select(y => y.vector.Length).ToList()).ToList();

            //Create a new list that contains max length of vector in each group.
            List<float> maxVectorLengthList = group.Select(x => x.Max(y => y.vector.Length)).ToList();
            //LogUtility.Instance.Log($"The max length of each group:", LogType.Debug);
            //foreach (float length in maxVectorLengthList)
            //{
            //    LogUtility.Instance.Log($"group {maxVectorLengthList.IndexOf(length)} has max length is {length}", LogType.Debug);
            //}

            float maxLength = maxVectorLengthList.OrderByDescending(x => x).First();
            //LogUtility.Instance.Log($"The max length: {maxLength} of the group {maxVectorLengthList.IndexOf(maxLength)}", LogType.Debug);

            //Choose the group of line which has max length of vector.
            List<Line> chosenLinesGroup = group[maxVectorLengthList.IndexOf(maxLength)];

            if (chosenLinesGroup.Count > 6)
            {
                ToleranceForDirectionLine = 0;
                for (int i = 0; i < chosenLinesGroup.Count; i++)
                {
                    List<Line> parallel = chosenLinesGroup.Where(x => Vector3.Parallel(chosenLinesGroup[i].vector, x.vector, ToleranceForDirectionLine) == true).ToList();
                    if (parallel.Count >= 3)
                    {
                        chosenLinesGroup = parallel;
                        break;
                    }
                }
            }


            Line directionLine = chosenLinesGroup.OrderByDescending(x => x.vector.Length).FirstOrDefault();

            return directionLine;
        }


        #region methods for detecting member type
        public bool Detect(out Vector3 p1, out Vector3 p2, out MemberPartType memberType)
        {
            p1 = p2 = default;
            memberType = MemberPartType.None;
            if (Mesh == null)
            {
                return false;
            }
            //1. Get direction of member
            //2. Project all points of mesh to OBB's start plane, also remove closed points => obtain a list of points on same plane
            //3. Rotate all points above to Oxy plane => obtain a list of points on same plane Oxy
            //4. Get the convex hull of list points on Oxy plane => obtain a list of points on convex hull
            //5. Remove nearest points and middle points of a linesegment
            //6. Detect the member type based on the list of points on convex hull
            //7. Get the expected point of member based on the member type
            //8. Transform the expected point to the Original (3D) coordinate system
            //9. Find the end point based on the expected point and the member's direction

            //Step1. 
            Plane plane = new Plane(_direction.Normalized, _start);
            ////Step2.
            List<Vector3> points = GetProjectPoints(plane, Mesh);

            //string path = $@"C:\Users\AnhTu\Member Structure Detection\MemberDetection\MemberDetection\Name_projetions.obj";
            //WritePointToObj(points, path);
            //Step3.
            MyQuaternion rotation = MyQuaternion.FromToRotation(_direction.Normalized, Vector3.Zaxis);
            if (Vector3.Parallel(_direction.Normalized, Vector3.Zaxis, SameAngleRadianTolerance))
            {
                rotation = MyQuaternion.Identity;
            }
            //LogUtility.Instance.Log($"Rotation 1: (w:{rotation.w}, x:{rotation.x}, y:{rotation.y}, z:{rotation.z}", LogType.Debug);
            //translate to origin based on start point
            points = points.Select(x => x - _start).ToList();
            //rotate to Oxy plane
            points = points.Select(x => Vector3.Transform(x, rotation)).ToList();
            //path = $@"C:\Users\AnhTu\Member Structure Detection\MemberDetection\MemberDetection\Name_rotation.obj";
            //WritePointToObj(points, path);

            //Step4.
            var convexHull = ConvexHull.GetConvexHull(points.Select(x => new Vector2(x.x, x.y)).ToList());
            //path = $@"C:\Users\AnhTu\Member Structure Detection\MemberDetection\MemberDetection\Name_convex_hull.obj";
            //WritePointToObj(convexHull.Select(x => new Vector3(x.x, x.y, 0)).ToList(), path);
            //Step5.
            RemoveMidlePointOnSegmentConvexHull(ref convexHull);
            //path = $@"C:\Users\AnhTu\Member Structure Detection\MemberDetection\MemberDetection\Name_convex_hull_removed.obj";
            //WritePointToObj(convexHull.Select(x => new Vector3(x.x, x.y, 0)).ToList(), path);
            //Step6.           
            memberType = GetMemberType(convexHull, out Vector2 point);
            //Step7. calculate 2 expected points
            return GetExpectedPoints(_direction, memberType, new Vector3(point.x, point.y, 0), out p1, out p2);
        }

        public static void WritePointToObj(List<Vector3> points, string objFile)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in points)
            {
                sb.AppendLine($"v {p.x} {p.y} {p.z}");
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                sb.AppendLine($"l {i + 1} {i + 2}");
            }
            File.WriteAllText(objFile, sb.ToString());
        }

        private bool GetExpectedPoints(Vector3 direction, MemberPartType memberType, Vector3 point, out Vector3 p1, out Vector3 p2)
        {
            p1 = p2 = default;
            if (memberType == MemberPartType.None)
            {
                return false;
            }
            //1. translate point to the original coordinate system
            MyQuaternion rotation = MyQuaternion.FromToRotation(Vector3.Zaxis, direction.Normalized);
            if (Vector3.Parallel(_direction.Normalized, Vector3.Zaxis, SameAngleRadianTolerance))
            {
                rotation = MyQuaternion.Identity;
            }
            //LogUtility.Instance.Log($"Rotation 2: (w:{rotation.w}, x:{rotation.x}, y:{rotation.y}, z:{rotation.z}", LogType.Debug);
            //2. find the end point based on the expected point and the member's direction
            p1 = Vector3.Transform(point, rotation) + _start;
            p2 = p1 + direction;
            return true;
        }

        private MemberPartType GetMemberType(List<Vector2> convexHull, out Vector2 point)
        {
            //LogUtility.Instance.Log($"Convex hull: {convexHull.Count}", LogType.Debug);
            point = default;
            if (convexHull.Count < 3)
                return MemberPartType.None;
            //1. get angle of each point in convex hull
            List<float> angles = new List<float>();
            for (int i = 0; i < convexHull.Count; i++)
            {
                var p1 = convexHull[i % convexHull.Count];
                var p2 = convexHull[(i + 1) % convexHull.Count];
                var p3 = convexHull[(i + 2) % convexHull.Count];
                var v1 = (p1 - p2).Normalized;
                var v2 = (p3 - p2).Normalized;
                var angle = Vector2.Angle(v1, v2);
                angles.Add(angle);
            }
            //LogUtility.Instance.Log($"angles: {string.Join(", ", angles)}", LogType.Debug);

            //2. get all linesegment of convex hull
            List<Segment> segments = new List<Segment>();
            for (int i = 0; i < convexHull.Count; i++)
            {
                var p1 = convexHull[i];
                var p2 = convexHull[(i + 1) % convexHull.Count];
                segments.Add(new Segment(new Vector3(p1.x, p1.y, 0), new Vector3(p2.x, p2.y, 0)));
            }

            segments = segments.OrderByDescending(x => x.Length).ToList();
            //LogUtility.Instance.Log($"segments: {string.Join(", ", segments.Select(x => x.Length))}", LogType.Debug);
            //3. detect member type
            var numAngle90 = angles.Count(x => x > 89 && x < 91);
            var numAngleGreater90 = angles.Count(x => x > 91);
            //LogUtility.Instance.Log($"numAngle90: {numAngle90}, numAngleGreater90: {numAngleGreater90}", LogType.Debug);
            //Angle type
            if (segments.Count == 5 && numAngle90 == 3 && numAngleGreater90 == 2)
            {
                var d1 = Vector2.Distance(segments[1].Start, segments[2].Start);
                var d2 = Vector2.Distance(segments[1].Start, segments[2].End);
                if (d1 < d2)
                {
                    point = new Vector2(segments[2].Start.x, segments[2].Start.y);
                }
                else
                {
                    point = new Vector2(segments[2].End.x, segments[2].End.y);
                }
                LogUtility.Instance.Log($"Angle Type", LogType.Debug);
                return MemberPartType.Angle;
            }
            //RoundBar
            else if (numAngle90 == 0 && numAngleGreater90 >= 16)
            {
                foreach (var p in convexHull)
                {
                    point += p;
                }
                point /= convexHull.Count;//Center point of convex hull
                LogUtility.Instance.Log($"Round Bar", LogType.Debug);
                return MemberPartType.RoundBar;
            }
            //SquareBar
            else if (numAngle90 == 4 && numAngleGreater90 == 0
                    && SameAngles(angles) && SameEdgeLength(segments))
            {
                foreach (var p in convexHull)
                {
                    point += p;
                }
                point /= convexHull.Count;//Center point of convex hull
                LogUtility.Instance.Log($"Square Bar", LogType.Debug);
                return MemberPartType.SquareBar;
            }
            LogUtility.Instance.Log($"None", LogType.Debug);
            return MemberPartType.None;
        }

        private bool SameEdgeLength(List<Segment> segments)
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                if (Math.Abs(segments[i].Length - segments[i + 1].Length) > SamePointDistanceTolerance)
                {
                    return false;
                }
            }
            return true;
        }

        private bool SameAngles(List<float> angles)
        {
            for (int i = 0; i < angles.Count - 1; i++)
            {
                if (Math.Abs(angles[i] - angles[i + 1]) > SameAngleDegreeTolerance)
                {
                    return false;
                }
            }
            return true;
        }

        private void RemoveMidlePointOnSegmentConvexHull(ref List<Vector2> convexHull)
        {
            List<int> removedIndexs = new List<int>();
            int inumPoint = convexHull.Count;
            for (int i = 0; i < inumPoint; i++)
            {
                var p1 = convexHull[i];
                var p2 = convexHull[(i + 1) % inumPoint];
                var p3 = convexHull[(i + 2) % inumPoint];
                var v1 = p2 - p1;
                var v2 = p3 - p2;
                if (Vector2.Angle(v1, v2) < SameAngleDegreeTolerance)
                {
                    removedIndexs.Add((i + 1) % inumPoint);
                }
            }
            //Remove midle point
            removedIndexs = removedIndexs.OrderByDescending(x => x).ToList();
            foreach (var index in removedIndexs)
            {
                convexHull.RemoveAt(index);
            }
        }

        //private List<Vector3> GetProjectPoints(Plane plane, Mesh mesh)
        //{
        //    List<Vector3> intersections = new List<Vector3>();
        //    foreach (var point in mesh.Points)
        //    {
        //        Line lineItem = new Line(vector: plane.Normal,
        //                                 pointFrom: point,
        //                                 pointTo: point);

        //        Vector3? intersection = plane.intersectionWithLine(lineItem);

        //        if (intersections.Any(x => Vector3.Distance(x, (Vector3)intersection) < SamePointDistanceTolerance))//2mm
        //        {
        //            continue;
        //        }

        //        if (intersection != null)
        //            intersections.Add((Vector3)intersection);
        //    }

        //    return intersections;
        //}

        private List<Vector3> GetProjectPoints(Plane plane, Mesh mesh)
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var p in mesh.Points)
            {
                var pp = plane.GetProjectionPoint(p);
                if (points.Any(x => Vector3.Distance(x, pp) < SamePointDistanceTolerance))//2mm
                {
                    continue;
                }
                points.Add(pp);
            }
            return points;
        }
        #endregion

    }
}
