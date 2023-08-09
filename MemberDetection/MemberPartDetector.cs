using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Intratech.Cores;

namespace MemberDetection
{
    public class MemberPartDetector
    {
        public bool detect(in DataGeometry dataGeometry, out MemberType memberType, out Vector3? firstCenter, out Vector3? secondCenter, out double? maxDistance)
        {
            List<Vector3> point3DsOnShape = dataGeometry.fromVerticesToPoint3D();
            List<LineItem> lineItems = dataGeometry.createLinesFromGeometry(points: point3DsOnShape,
                                                                            faces: dataGeometry.Faces);

            DirectionLineItem getDirectionVector = new DirectionLineItem(lineItems);
            LineItem directionVector = getDirectionVector.getDirectionLineItem();

            PlaneItem planeItem = new PlaneItem(normalVector: directionVector.vector,
                                                point3D: new Vector3(0, 0, 0));

            List<Vector2> point2DsShapeOnPlane = planeItem.get2DProjectionPointsOnPlane(point3DsOnShape);

            ListPoints listPoints = new ListPoints(point2Ds: point2DsShapeOnPlane);
            List<Vector2> list2DPointsRemoved = listPoints.removeNearest2DPoints();

            ConvexHul convexHul = new ConvexHul(list2DPointsRemoved.ToArray());
            List<Vector2> pointsOnHull = convexHul.GetConvexHull(list2DPointsRemoved);

            listPoints.Point2Ds = pointsOnHull;
            List<Vector2> pointsOnHullRemoved = listPoints.removeNearest2DPoints();

            using (StreamWriter writer = new StreamWriter("C:\\Users\\AnhTu\\Member Structure Detection\\MemberDetection\\MemberDetection\\PointOnHull.txt"))
            {
                foreach (Vector2 point in pointsOnHullRemoved)
                {
                    writer.WriteLine($"({point.x},{point.y})");
                }
            }

            DetectType detectMemberType = new DetectType(pointsOnHullRemoved);
            memberType = detectMemberType.detect();
            MessageBox.Show($"The type of selected item is {memberType.ToString()}");

            maxDistance = 0;
            if (memberType == MemberType.squareBar || memberType == MemberType.roundBar)
            {
                SquareRoundBarParameters squareRoundBarParameters = new SquareRoundBarParameters(pointsOnHull: pointsOnHullRemoved,
                                                                                                 planeItem: planeItem,
                                                                                                 point3DsOnShape: point3DsOnShape);

                squareRoundBarParameters.calculate(out firstCenter, out secondCenter);

                MessageBox.Show($"The center points of shape are:\n({firstCenter.Value.x}, {firstCenter.Value.y}, {firstCenter.Value.z})\n({secondCenter.Value.x}, {secondCenter.Value.y}, {secondCenter.Value.z})");
            }
            else if (memberType == MemberType.angleType)
            {
                AngleTypeParameters angleTypeParameters = new AngleTypeParameters(pointsOnHull: pointsOnHullRemoved,
                                                                                  planeItem: planeItem,
                                                                                  point3DsOnShape: point3DsOnShape,
                                                                                  plane: planeItem);
                angleTypeParameters.calculate(out firstCenter, out secondCenter, out maxDistance);

                MessageBox.Show($"The center points of shape are:\n({firstCenter.Value.x}, {firstCenter.Value.y}, {firstCenter.Value.z})\n({secondCenter.Value.x}, {secondCenter.Value.y}, {secondCenter.Value.z})\nThe max length of shape is: {(firstCenter - secondCenter)?.Length}");
            }
            else
            {
                MessageBox.Show("This is not Square Bar, Round Bar and Angle Type");
                firstCenter = null;
                secondCenter = null;
                maxDistance = null;
                return false;
            }

            return true;
        }

        public MemberType getType(DataGeometry dataGeometry)
        {
            List<Vector3> point3DsOnShape = dataGeometry.fromVerticesToPoint3D();
            List<LineItem> lineItems = dataGeometry.createLinesFromGeometry(points: point3DsOnShape,
                                                                            faces: dataGeometry.Faces);

            DirectionLineItem getDirectionVector = new DirectionLineItem(lineItems);
            LineItem directionVector = getDirectionVector.getDirectionLineItem();

            PlaneItem planeItem = new PlaneItem(normalVector: directionVector.vector,
                                                point3D: new Vector3(0, 0, 0));

            List<Vector2> point2DsShapeOnPlane = planeItem.get2DProjectionPointsOnPlane(point3DsOnShape);

            ListPoints listPoints = new ListPoints(point2Ds: point2DsShapeOnPlane);
            List<Vector2> list2DPointsRemoved = listPoints.removeNearest2DPoints();

            ConvexHul convexHul = new ConvexHul(list2DPointsRemoved.ToArray());
            List<Vector2> pointsOnHull = convexHul.GetConvexHull(list2DPointsRemoved);

            listPoints.Point2Ds = pointsOnHull;
            List<Vector2> pointsOnHullRemoved = listPoints.removeNearest2DPoints();

            DetectType detectMemberType = new DetectType(pointsOnHullRemoved);
            MemberType memberType = detectMemberType.detect();
            return memberType;
        }
    }
}
