using Intratech.Cores;
using Intratech.Cores.Geometries;
using System;
using System.Collections.Generic;

namespace Intra.GeometryDetection
{
    public class MemberPartDetector
    {
        public bool detect(in Mesh mesh, out MemberType memberType, out Vector3? firstCenter, out Vector3? secondCenter, out double? maxDistance)
        {
            DataGeometry dataGeometry = new DataGeometry(vertices: mesh.Points,
                                                         faces: mesh.Faces);

            List<LineItem> lineItems = dataGeometry.createLinesFromTriangles();

            DirectionLineItem getDirectionVector = new DirectionLineItem(lineItems);
            LineItem directionVector = getDirectionVector.getDirectionLineItem();

            PlaneItem planeItem = new PlaneItem(normalVector: directionVector.vector,
                                                point3D: new Vector3(0, 0, 0));

            ListPoints listPoints = new ListPoints(point3Ds: mesh.Points);
            List<Vector3> point3DsOnShape = listPoints.removeNearest3DPoints();
            List<Vector2> point2DsShapeOnPlane = planeItem.get2DProjectionPointsOnPlane(point3DsOnShape);

            listPoints.Point2Ds = point2DsShapeOnPlane;
            List<Vector2> list2DPointsRemoved = listPoints.removeNearest2DPoints();

            ConvexHul convexHul = new ConvexHul();
            List<Vector2> pointsOnHull = convexHul.GetConvexHull(list2DPointsRemoved);

            listPoints.Point2Ds = pointsOnHull;
            List<Vector2> pointsOnHullRemoved = listPoints.removeNearest2DPoints();

            DetectType detectMemberType = new DetectType(pointsOnHullRemoved);
            memberType = detectMemberType.detect();

            maxDistance = 0;
            if (memberType == MemberType.squareBar || memberType == MemberType.roundBar)
            {
                SquareRoundBarParameters squareRoundBarParameters = new SquareRoundBarParameters(pointsOnHull: pointsOnHullRemoved,
                                                                                                 planeItem: planeItem,
                                                                                                 point3DsOnShape: mesh.Points);
                squareRoundBarParameters.calculate(out firstCenter, out secondCenter);
            }
            else if (memberType == MemberType.angleType)
            {
                AngleTypeParameters angleTypeParameters = new AngleTypeParameters(pointsOnHull: pointsOnHullRemoved,
                                                                                  point2DsShapeOnPlane: point2DsShapeOnPlane,
                                                                                  point3DsOnShape: point3DsOnShape);

                angleTypeParameters.calculate(out firstCenter, out secondCenter, out maxDistance);
            }
            else
            {
                firstCenter = null;
                secondCenter = null;
                return false;
            }

            return true;
        }
    }
}
