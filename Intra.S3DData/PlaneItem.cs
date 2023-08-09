using Intratech.Cores;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Intra.GeometryDetection
{
    public class PlaneItem
    {
        public Vector3 normalVector { get; set; }
        public Vector3 point3D { get; set; }

        public PlaneItem(Vector3 normalVector, Vector3 point3D)
        {
            this.normalVector = normalVector;
            this.point3D = point3D;
        }

        public Vector3? crossPointWithLine(LineItem line)
        {
            Vector3? crossPoint = null;
            Vector3 normalVectorPlane = this.normalVector;
            Vector3 normalVectorLine = line.vector;
            double denominator = Vector3.Dot(normalVectorPlane, normalVectorLine);

            if (denominator != 0)
            {
                var a0 = this.normalVector.x;
                var b0 = this.normalVector.y;
                var c0 = this.normalVector.z;

                var a1 = line.vector.x;
                var b1 = line.vector.y;
                var c1 = line.vector.z;

                var x0 = this.point3D.x;
                var y0 = this.point3D.y;
                var z0 = this.point3D.z;

                var x1 = line.pointFrom.Value.x;
                var y1 = line.pointFrom.Value.y;
                var z1 = line.pointFrom.Value.z;

                var t = -(a0 * (x1 - x0) + b0 * (y1 - y0) + c0 * (z1 - z0)) / denominator;
                crossPoint = new Vector3(x: x1 + a1 * t,
                                         y: y1 + b1 * t,
                                         z: z1 + c1 * t);
            }

            return crossPoint;
        }

        public List<Vector3?> get3DProjectionPointsOnPlane(List<Vector3> points)
        {
            List<Vector3?> projectionPoints = new List<Vector3?>();
            foreach (var point in points)
            {
                LineItem lineItem = new LineItem(vector: this.normalVector,
                                                 pointFrom: point,
                                                 pointTo: null);

                Vector3? crossPoint = this.crossPointWithLine(lineItem);
                projectionPoints.Add(crossPoint);
            }

            return projectionPoints;
        }

        public List<Vector2> get2DProjectionPointsOnPlane(List<Vector3> points)
        {
            List<Vector3?> intersections = new List<Vector3?>();
            foreach (var point in points)
            {
                LineItem lineItem = new LineItem(vector: this.normalVector,
                                                 pointFrom: point,
                                                 pointTo: null);

                Vector3? intersection = this.crossPointWithLine(lineItem);
                intersections.Add(intersection);
            }

            List<Vector2> pointsShapeOnPlane = this.convert3DTo2D(intersections);

            return pointsShapeOnPlane;
        }

        public List<Vector2> convert3DTo2D(List<Vector3?> points)
        {
            Vector3 z_axis = this.normalVector;

            Vector3 y_axis = new Vector3(0, 0, 0);
            if (z_axis.x != 0)
            {
                y_axis = new Vector3(x: -(z_axis.y / z_axis.x),
                                      y: 1,
                                      z: 0);
            }
            else if (z_axis.x == 0)
            {
                y_axis = new Vector3(x: 1,
                                      y: 0,
                                      z: 0);
            }

            var b1 = z_axis.x; var b2 = z_axis.y; var b3 = z_axis.z;
            var a1 = y_axis.x; var a2 = y_axis.y; var a3 = y_axis.z;

            Vector3 x_axis = new Vector3(x: a2 * b3 - a3 * b2,
                                           y: a3 * b1 - a1 * b3,
                                           z: a1 * b2 - a2 * b1);

            x_axis.Normalize();
            y_axis.Normalize();
            z_axis.Normalize();

            var Ox = new Vector3(1, 0, 0); var Oy = new Vector3(0, 1, 0); var Oz = new Vector3(0, 0, 1);

            var rotationMatrix = new System.Windows.Media.Media3D.Matrix3D();
            rotationMatrix.M11 = (double)Vector3.Dot(x_axis, Ox);
            rotationMatrix.M12 = (double)Vector3.Dot(y_axis, Ox);
            rotationMatrix.M13 = (double)Vector3.Dot(z_axis, Ox);

            rotationMatrix.M21 = (double)Vector3.Dot(x_axis, Oy);
            rotationMatrix.M22 = (double)Vector3.Dot(y_axis, Oy);
            rotationMatrix.M23 = (double)Vector3.Dot(z_axis, Oy);

            rotationMatrix.M31 = (double)Vector3.Dot(x_axis, Oz);
            rotationMatrix.M32 = (double)Vector3.Dot(y_axis, Oz);
            rotationMatrix.M33 = (double)Vector3.Dot(z_axis, Oz);

            List<Vector3> rotationPoints = new List<Vector3>();
            List<Point3D> point3Ds = new List<Point3D>();
            foreach (Vector3 point in points)
            {
                Point3D newPoint3D = new Point3D(x: point.x,
                                                 y: point.y,
                                                 z: point.z);
                point3Ds.Add(newPoint3D);
            }

            List<Point3D> rotationPoint3Ds = new List<Point3D>();
            foreach (Point3D point in point3Ds)
            {
                Point3D rotationPoint = rotationMatrix.Transform(point);
                rotationPoint3Ds.Add(rotationPoint);
            }

            foreach (Point3D point in rotationPoint3Ds)
            {
                Vector3 newPoint = new Vector3(x: point.X,
                                               y: point.Y,
                                               z: point.Z);
                rotationPoints.Add(newPoint);
            }

            List<Vector2> list2DPoints = new List<Vector2>();
            foreach (Vector3 rotationPoint in rotationPoints)
            {
                Vector2 planePoint = new Vector2(x: rotationPoint.x,
                                                 y: rotationPoint.y);
                list2DPoints.Add(planePoint);
            }

            return list2DPoints;
        }

        public List<Vector3> convert2DTo3D(List<Vector2> points)
        {
            Vector3 z_axis = this.normalVector;

            Vector3 y_axis = new Vector3(0, 0, 0);
            if (z_axis.x != 0)
            {
                y_axis = new Vector3(x: -(z_axis.y / z_axis.x),
                                      y: 1,
                                      z: 0);
            }
            else
            {
                y_axis = new Vector3(x: 1,
                                      y: 0,
                                      z: 0);
            }

            var b1 = z_axis.x; var b2 = z_axis.y; var b3 = z_axis.z;
            var a1 = y_axis.x; var a2 = y_axis.y; var a3 = y_axis.z;

            Vector3 x_axis = new Vector3(x: a2 * b3 - a3 * b2,
                                           y: a3 * b1 - a1 * b3,
                                           z: a1 * b2 - a2 * b1);

            x_axis.Normalize();
            y_axis.Normalize();
            z_axis.Normalize();

            var Ox = new Vector3(1, 0, 0); var Oy = new Vector3(0, 1, 0); var Oz = new Vector3(0, 0, 1);

            var rotationMatrix = new System.Windows.Media.Media3D.Matrix3D();
            rotationMatrix.M11 = (double)Vector3.Dot(x_axis, Ox);
            rotationMatrix.M12 = (double)Vector3.Dot(y_axis, Ox);
            rotationMatrix.M13 = (double)Vector3.Dot(z_axis, Ox);

            rotationMatrix.M21 = (double)Vector3.Dot(x_axis, Oy);
            rotationMatrix.M22 = (double)Vector3.Dot(y_axis, Oy);
            rotationMatrix.M23 = (double)Vector3.Dot(z_axis, Oy);

            rotationMatrix.M31 = (double)Vector3.Dot(x_axis, Oz);
            rotationMatrix.M32 = (double)Vector3.Dot(y_axis, Oz);
            rotationMatrix.M33 = (double)Vector3.Dot(z_axis, Oz);
            rotationMatrix.Invert();

            List<Vector3> point3DInputs = new List<Vector3>();
            foreach (Vector2 pointItem in points)
            {
                Vector3 newVector3 = new Vector3(x: pointItem.x,
                                                 y: pointItem.y,
                                                 z: 0);
                point3DInputs.Add(newVector3);
            }

            List<Vector3> rotationPoints = new List<Vector3>();
            List<Point3D> point3Ds = new List<Point3D>();

            foreach (Vector3 point in point3DInputs)
            {
                Point3D newPoint3D = new Point3D(x: point.x,
                                                 y: point.y,
                                                 z: point.z);
                point3Ds.Add(newPoint3D);
            }

            List<Point3D> rotationPoint3Ds = new List<Point3D>();
            foreach (Point3D point3D in point3Ds)
            {
                Point3D rotationPoint = rotationMatrix.Transform(point3D);
                rotationPoint3Ds.Add(rotationPoint);
            }

            foreach (Point3D point in rotationPoint3Ds)
            {
                Vector3 newPoint = new Vector3(x: point.X,
                                               y: point.Y,
                                               z: point.Z);
                rotationPoints.Add(newPoint);
            }

            return rotationPoints;
        }
    }
}
