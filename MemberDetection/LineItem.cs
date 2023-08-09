using Intratech.Cores;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace MemberDetection
{
    public class LineItem
    {
        public Vector3 vector { get; set; }
        public Vector3? pointFrom { get; set; }
        public Vector3? pointTo { get; set; }

        public LineItem(Vector3 vector, Vector3? pointFrom, Vector3? pointTo)
        {
            this.vector = vector;
            this.pointFrom = pointFrom;
            this.pointTo = pointTo;
        }

        public LineItem(Vector3 pointFrom, Vector3 pointTo)
        {
            this.pointFrom = pointFrom;
            this.pointTo = pointTo;
            this.vector = new Vector3(x: (double)pointTo.x - (double)pointFrom.x,
                                      y: (double)pointTo.y - (double)pointFrom.y,
                                      z: (double)pointTo.z - (double)pointFrom.z);
        }


        public List<Vector3> findProjectPointsOnLine(List<Vector3> points)
        {
            List<Vector3> projectionPoints = new List<Vector3>();
            foreach (var point in points)
            {
                var a1 = point.x; var a2 = point.y; var a3 = point.z;
                var u1 = this.vector.x; var u2 = this.vector.y; var u3 = this.vector.z;
                var x0 = this.pointFrom.Value.x; var y0 = this.pointFrom.Value.y; var z0 = this.pointFrom.Value.z;

                var t = -(u1 * (x0 - a1) + u2 * (y0 - a2) + u3 * (z0 - a3)) / (u1 * u1 + u2 * u2 + u3 * u3);

                Vector3 projectionPoint = new Vector3(x: x0 + u1 * t,
                                                      y: y0 + u2 * t,
                                                      z: z0 + u3 * t);

                projectionPoints.Add(projectionPoint);
            }

            return projectionPoints;
        }
    }

    public class ListLines
    {
        public List<LineItem> Lines {  get; set; } = new List<LineItem>();

        public ListLines()
        {
        }

        public ListLines(List<Vector2> points)
        {
            List<LineItem> listLines = new List<LineItem>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                LineItem lineItem = new LineItem(pointFrom: new Vector3((double)points[i].x, (double)points[i].y, 0),
                                                 pointTo: new Vector3((double)points[i + 1].x, (double)points[i + 1].y, 0));
                lineItem.vector.Normalize();
                listLines.Add(lineItem);
            }
            
            LineItem lastLineItem = new LineItem(pointFrom: new Vector3((double)points[points.Count - 1].x, (double)points[points.Count - 1].y, 0),
                                                 pointTo: new Vector3((double)points[0].x, (double)points[0].y, 0));
            lastLineItem.vector.Normalize();
            listLines.Add(lastLineItem);

            this.Lines = listLines;
        }

        public ListLines removedLineParallel()
        {
            ListLines removedListLineItems = new ListLines();
            List<LineItem> removedListLines = new List<LineItem>() { this.Lines[0] };
            for (int i = 1; i < this.Lines.Count; i++)
            {
                LineItem lastLine = removedListLines.Last();
                //All vectors in List Lines are both continuous and consecutive.
                if (Vector3.Angle(lastLine.vector, this.Lines[i].vector) <= 0.1)
                {
                    //Two vector are the same direction, we expand the last vector to the pointTo of considered line.
                    LineItem lineItem = new LineItem(pointFrom: lastLine.pointFrom.Value,
                                                     pointTo: this.Lines[i].pointTo.Value);

                    removedListLines[removedListLines.Count - 1] = lineItem;
                }
                else if (Vector3.Angle(lastLine.vector, this.Lines[i].vector) >= 179.9)
                {
                    //Two vector are the opposite direction, we expand the last vector to the pointFrom of considered line.
                    LineItem lineItem = new LineItem(pointFrom: lastLine.pointFrom.Value,
                                                     pointTo: this.Lines[i].pointFrom.Value);

                    removedListLines[removedListLines.Count - 1] = lineItem;
                }
                else
                {
                    removedListLines.Add(this.Lines[i]);
                }
            }

            //Execute the last vector and the intial vector.
            if (Vector3.Angle(removedListLines.Last().vector, this.Lines[0].vector) == 0)
            {
                LineItem lineItem = new LineItem(pointFrom: removedListLines.Last().pointFrom.Value,
                                                 pointTo: this.Lines[0].pointTo.Value);
                removedListLines[0] = lineItem;
                removedListLines.RemoveAt(removedListLines.Count - 1);
            }
            else if (Vector3.Angle(removedListLines.Last().vector, this.Lines[0].vector) == 180)
            {
                LineItem lineItem = new LineItem(pointFrom: removedListLines.Last().pointFrom.Value,
                                                 pointTo: this.Lines[0].pointFrom.Value);
                removedListLines[0] = lineItem;
                removedListLines.RemoveAt(removedListLines.Count - 1);
            }

            removedListLineItems.Lines = removedListLines;
            return removedListLineItems;
        }

        public List<double> listAngleBetweenLines()
        {
            List<double> angles = new List<double>();
            double firstAngle = Vector3.Angle(Lines[Lines.Count - 1].vector, Lines[0].vector);
            angles.Add(firstAngle);

            for (int i = 0; i < Lines.Count - 1; i++)
            {
                double angle = Vector3.Angle(Lines[i].vector, Lines[i + 1].vector);
                angles.Add(angle);
            }
            return angles;
        }
    }
}
