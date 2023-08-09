using Intratech.Cores;
using System.Collections.Generic;

namespace Intra.MemberDetector
{
    public class Plane
    {
        public Vector3 Normal { get; }
        public Vector3 Origin { get; }
        public float MDistance { get; }

        public Plane(Vector3 normal, Vector3 origin)
        {
            Normal = normal;
            Origin = origin;
            MDistance = -Vector3.Dot(Normal, Origin);
        }

        public double DistanceTo(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + MDistance;
        }

        public Vector3 GetProjectionPoint(Vector3 point)
        {
            return point - Vector3.Dot(point - Origin, Normal) * Normal;
        }

        public static double DistanceToPlane(Vector3 normal, Vector3 origin, Vector3 point)
        {
            var m_Distance = -Vector3.Dot(normal, origin);
            return Vector3.Dot(normal, point) + m_Distance;
        }

        public Vector3? intersectionWithLine(Line line)
        {
            Vector3? intersectionPoint = null;
            Vector3 normalVectorPlane = this.Normal;
            Vector3 normalVectorLine = line.vector;
            double denominator = Vector3.Dot(normalVectorPlane, normalVectorLine);

            if (denominator != 0)
            {
                var a0 = this.Normal.x;
                var b0 = this.Normal.y;
                var c0 = this.Normal.z;

                var a1 = line.vector.x;
                var b1 = line.vector.y;
                var c1 = line.vector.z;

                var x0 = this.Origin.x;
                var y0 = this.Origin.y;
                var z0 = this.Origin.z;

                var x1 = line.pointFrom.x;
                var y1 = line.pointFrom.y;
                var z1 = line.pointFrom.z;

                var t = -(a0 * (x1 - x0) + b0 * (y1 - y0) + c0 * (z1 - z0)) / denominator;
                intersectionPoint = new Vector3(x: x1 + a1 * t,
                                         y: y1 + b1 * t,
                                         z: z1 + c1 * t);
            }

            return intersectionPoint;
        }
    }
}