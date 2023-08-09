using System;
using System.Collections.Generic;

namespace MemberDetection
{
    public class CallbackGeomListener : Autodesk.Navisworks.Api.Interop.ComApi.InwSimplePrimitivesCB
    {
        public Autodesk.Navisworks.Api.Interop.ComApi.InwLTransform3f3 LCS2WCS;
        public List<System.Windows.Media.Media3D.Point3D> vertices = new List<System.Windows.Media.Media3D.Point3D>();
        public List<int> faces = new List<int>();
        public System.Windows.Media.Media3D.Matrix3D matrix = new System.Windows.Media.Media3D.Matrix3D();
        public Dictionary<System.Windows.Media.Media3D.Point3D, int> addedVertices = new Dictionary<System.Windows.Media.Media3D.Point3D, int>();

        public void Line(Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v1, Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v2)
        {
            // do your work
        }

        public void Point(Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v1)
        {
            // do your work
        }

        public void SnapPoint(Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v1)
        {
            // do your work
        }

        public void Triangle(Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v1,
                             Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v2,
                             Autodesk.Navisworks.Api.Interop.ComApi.InwSimpleVertex v3)
        {
            // do your work
            Array array_v1 = (Array)(object)v1.coord;
            Array array_v2 = (Array)(object)v2.coord;
            Array array_v3 = (Array)(object)v3.coord;

            System.Windows.Media.Media3D.Point3D vertice1 = new System.Windows.Media.Media3D.Point3D(x: Convert.ToDouble(array_v1.GetValue(1)),
                                                                                                     y: Convert.ToDouble(array_v1.GetValue(2)),
                                                                                                     z: Convert.ToDouble(array_v1.GetValue(3)));
            vertice1 = matrix.Transform(vertice1);

            System.Windows.Media.Media3D.Point3D vertice2 = new System.Windows.Media.Media3D.Point3D(x: Convert.ToDouble(array_v2.GetValue(1)),
                                                                                                     y: Convert.ToDouble(array_v2.GetValue(2)),
                                                                                                     z: Convert.ToDouble(array_v2.GetValue(3)));
            vertice2 = matrix.Transform(vertice2);

            System.Windows.Media.Media3D.Point3D vertice3 = new System.Windows.Media.Media3D.Point3D(x: Convert.ToDouble(array_v3.GetValue(1)),
                                                                                                     y: Convert.ToDouble(array_v3.GetValue(2)),
                                                                                                     z: Convert.ToDouble(array_v3.GetValue(3)));
            vertice3 = matrix.Transform(vertice3);

            faces.Add(addVertex(vertice1));
            faces.Add(addVertex(vertice2));
            faces.Add(addVertex(vertice3));
        }

        public int addVertex(System.Windows.Media.Media3D.Point3D vertice)
        {
            if (!addedVertices.ContainsKey(vertice))
            {
                vertices.Add(vertice);
                addedVertices[vertice] = vertices.Count - 1;
            }

            return addedVertices[vertice];
        }
    }
}
