using Intratech.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intra.MemberDetector
{
    public class Line
    {
        public Vector3 vector { get; set; }
        public Vector3 pointFrom { get; set; }
        public Vector3 pointTo { get; set; }

        public Line(Vector3 vector, Vector3 pointFrom, Vector3 pointTo)
        {
            this.vector = vector;
            this.pointFrom = pointFrom;
            this.pointTo = pointTo;
        }

        public Line(Vector3 pointFrom, Vector3 pointTo)
        {
            this.pointFrom = pointFrom;
            this.pointTo = pointTo;
            this.vector = new Vector3(x: (double)pointTo.x - (double)pointFrom.x,
                                      y: (double)pointTo.y - (double)pointFrom.y,
                                      z: (double)pointTo.z - (double)pointFrom.z);
        }

        public Vector3 projectionPointOnLine(Vector3 point)
        {
            var a1 = point.x; var a2 = point.y; var a3 = point.z;
            var u1 = this.vector.x; var u2 = this.vector.y; var u3 = this.vector.z;
            var x0 = this.pointFrom.x; var y0 = this.pointFrom.y; var z0 = this.pointFrom.z;

            var t = -(u1 * (x0 - a1) + u2 * (y0 - a2) + u3 * (z0 - a3)) / (u1 * u1 + u2 * u2 + u3 * u3);

            Vector3 projectionPoint = new Vector3(x: x0 + u1 * t,
                                                  y: y0 + u2 * t,
                                                  z: z0 + u3 * t);
            return projectionPoint;
        }
    }
}
