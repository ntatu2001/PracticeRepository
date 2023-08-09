using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Interop.ComApi;
using Intratech.Cores;
using System;
using System.Collections.Generic;
using COMApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;
using Intratech.Cores;

namespace MemberDetection
{
    [Serializable]
    public class VertexFragment
    {
        public double VertexX { get; set; }
        public double VertexY { get; set; }
        public double VertexZ { get; set; }

        public VertexFragment(double vertexX, double vertexY, double vertexZ)
        {
            VertexX = vertexX;
            VertexY = vertexY;
            VertexZ = vertexZ;
        }
    }

    [Serializable]
    public class GeometryColor
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double Transparent { get; set; }
    }

    [Serializable]
    public class DataGeometry
    {
        public List<VertexFragment> Vertices { get; set; }
        public List<int[]> Faces { get; set; }
        public GeometryColor Color { get; set; }

        public DataGeometry()
        {
        }

        public DataGeometry(List<VertexFragment> vertices, List<int[]> faces, GeometryColor color)
        {
            Vertices = vertices;
            Faces = faces;
            Color = color;
        }

        public static DataGeometry convertToDataGeometry(Intratech.Cores.Geometries.Mesh mesh)
        {
            List<VertexFragment> vertices = new List<VertexFragment>();
            foreach (Vector3 vector3 in mesh.Points)
            {
                VertexFragment vertexFragment = new VertexFragment(vertexX: vector3.x,
                                                                   vertexY: vector3.y,
                                                                   vertexZ: vector3.z);
                vertices.Add(vertexFragment);
            }

            List<int[]> faces = new List<int[]>();
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                if (i % 3 == 0)
                {
                    int[] face = new int[3];
                    face[0] = mesh.Faces[i];
                    face[1] = mesh.Faces[i + 1];
                    face[2] = mesh.Faces[i + 2];
                    faces.Add(face);
                }
            }

            DataGeometry dataGeometry = new DataGeometry(vertices, faces, null);
            return dataGeometry;
        }

        public DataGeometry getGeometry(ModelItemCollection modelItems)
        {
            if (modelItems != null)
            {
                COMApi.InwOpState oState = ComBridge.State;
                COMApi.InwOpSelection oSel = ComBridge.ToInwOpSelection(modelItems);

                // create the callback object
                CallbackGeomListener callbkListener = new CallbackGeomListener();
                callbkListener.addedVertices = new Dictionary<System.Windows.Media.Media3D.Point3D, int>();

                foreach (COMApi.InwOaPath3 path in oSel.Paths())
                {
                    foreach (COMApi.InwOaFragment3 frag in path.Fragments())
                    {
                        // generate the primitives
                        var localToWorld = (InwLTransform3f3)(object)frag.GetLocalToWorldMatrix();

                        var localToWorldMatrix = (Array)(object)localToWorld.Matrix;
                        callbkListener.matrix = this.getVerticesTransformationMatrix(localToWorldMatrix);

                        frag.GenerateSimplePrimitives(COMApi.nwEVertexProperty.eNORMAL, callbkListener);
                    }
                }

                List<VertexFragment> vertices = new List<VertexFragment>();
                List<int[]> faces = new List<int[]>();
                foreach (System.Windows.Media.Media3D.Point3D vertex in callbkListener.addedVertices.Keys)
                {
                    VertexFragment vertexFragment = new VertexFragment(vertexX: vertex.X,
                                                                       vertexY: vertex.Y,
                                                                       vertexZ: vertex.Z);
                    vertices.Add(vertexFragment);
                }

                for (int i = 0; i < callbkListener.faces.Count; i++)
                {
                    if (i % 3 == 0)
                    {
                        int[] face = new int[3];
                        face[0] = callbkListener.faces[i];
                        face[1] = callbkListener.faces[i + 1];
                        face[2] = callbkListener.faces[i + 2];
                        faces.Add(face);
                    }
                }

                DataGeometry dataGeometry = new DataGeometry(vertices: vertices, faces: faces, color: null);
                return dataGeometry;
            }
            return null;
        }

        public System.Windows.Media.Media3D.Matrix3D getVerticesTransformationMatrix(Array localToWorldMatrix)
        {
            var matrix = new System.Windows.Media.Media3D.Matrix3D();

            matrix.M11 = (double)localToWorldMatrix.GetValue(1);
            matrix.M12 = (double)localToWorldMatrix.GetValue(2);
            matrix.M13 = (double)localToWorldMatrix.GetValue(3);
            matrix.M14 = (double)localToWorldMatrix.GetValue(4);

            matrix.M21 = (double)localToWorldMatrix.GetValue(5);
            matrix.M22 = (double)localToWorldMatrix.GetValue(6);
            matrix.M23 = (double)localToWorldMatrix.GetValue(7);
            matrix.M24 = (double)localToWorldMatrix.GetValue(8);

            matrix.M31 = (double)localToWorldMatrix.GetValue(9);
            matrix.M32 = (double)localToWorldMatrix.GetValue(10);
            matrix.M33 = (double)localToWorldMatrix.GetValue(11);
            matrix.M34 = (double)localToWorldMatrix.GetValue(12);

            matrix.OffsetX = (double)localToWorldMatrix.GetValue(13);
            matrix.OffsetY = (double)localToWorldMatrix.GetValue(14);
            matrix.OffsetZ = (double)localToWorldMatrix.GetValue(15);
            matrix.M44 = (double)localToWorldMatrix.GetValue(16);
            return matrix;
        }

        public List<Vector3> fromVerticesToPoint3D()
        {
            List<Vector3> result = new List<Vector3>();
            foreach (VertexFragment vertex in this.Vertices)
            {
                result.Add(new Vector3(x: vertex.VertexX,
                                       y: vertex.VertexY,
                                       z: vertex.VertexZ));
            }

            return result;
        }

        public List<LineItem> createLinesFromGeometry(List<Vector3> points, List<int[]> faces)
        {
            Dictionary<int, List<int>> lineDictionary = new Dictionary<int, List<int>>();
            for (int i = 0; i < points.Count; i++)
            {
                lineDictionary.Add(i, new List<int>());
            }

            List<LineItem> lineItems = new List<LineItem>();
            foreach (int[] face in faces)
            {
                if (!lineDictionary[face[0]].Contains(face[1]))
                {
                    LineItem lineItem = new LineItem(points[face[1]], points[face[0]]);
                    lineItems.Add(lineItem);
                    lineDictionary[face[0]].Add(face[1]);
                    lineDictionary[face[1]].Add(face[0]);
                }

                if (!lineDictionary[face[0]].Contains(face[2]))
                {
                    LineItem lineItem = new LineItem(points[face[0]], points[face[2]]);
                    lineItems.Add(lineItem);
                    lineDictionary[face[0]].Add(face[2]);
                    lineDictionary[face[2]].Add(face[0]);
                }

                if (!lineDictionary[face[1]].Contains(face[2]))
                {
                    LineItem lineItem = new LineItem(points[face[1]], points[face[2]]);
                    lineItems.Add(lineItem);
                    lineDictionary[face[1]].Add(face[2]);
                    lineDictionary[face[2]].Add(face[1]);
                }
            }

            return lineItems;
        }
    }
}
