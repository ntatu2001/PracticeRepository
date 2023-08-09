using Autodesk.Navisworks.Api;
using Intratech.Cores;
using Intratech.PDF;
using Intratech.PRC;
using Intratech.PRC.API;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NavigationTools = Intratech.PDF._3D.NavigationTools;
using View = Intratech.PDF.Graphics.View;
using ViewDirection = Intratech.PDF.ViewUtility.ViewDirection;

namespace MemberDetection
{
    public class WriteToPDF
    {
        public static void recursive(Writer prcWriter, ModelItem currentItem, DataGeometry dataGeometry, List<Vector3?> centerPoints)
        {
            prcWriter.Tree.OpenNode(currentItem.DisplayName);
            WriteToPDF.writeGeometry(prcWriter, currentItem, dataGeometry, centerPoints);
            if (currentItem.Children != null)
            {
                foreach (ModelItem childItem in currentItem.Children)
                {
                    recursive(prcWriter, childItem, dataGeometry, centerPoints);
                }
            }
            prcWriter.Tree.CloseNode();
        }

        private static void writeGeometry(Writer prcWriter, ModelItem currentItem ,DataGeometry dataGeometry, List<Vector3?> centerPoints)
        {
            List<Vector3> pointsOnShape = dataGeometry.fromVerticesToPoint3D();

            List<float> pointList = new List<float>();
            List<int> faceList = new List<int>();
            float[] pointArray = null;
            int[] faceArray = null;

            if (currentItem.Geometry != null)
            {
                foreach (VertexFragment vertexFragment in dataGeometry.Vertices)
                {
                    pointList.Add((float)vertexFragment.VertexX);
                    pointList.Add((float)vertexFragment.VertexY);
                    pointList.Add((float)vertexFragment.VertexZ);
                }
                pointArray = pointList.ToArray();

                foreach (int[] face in dataGeometry.Faces)
                {
                    faceList.Add(face[0]);
                    faceList.Add(face[1]);
                    faceList.Add(face[2]);
                }
                faceArray = faceList.ToArray();

                Intratech.PRC.Material3D color = Intratech.PRC.Material3D.Default;
                color.Diffuse = new Intratech.PRC.RGBAColor(_r: currentItem.Geometry.ActiveColor.R,
                                                            _g: currentItem.Geometry.ActiveColor.G,
                                                            _b: currentItem.Geometry.ActiveColor.B);

                color.Transparent = 1 - (float)currentItem.Geometry.ActiveTransparency;
                prcWriter.Geometry.AddMesh(points: pointArray, faces: faceArray, mat: color, name: "", transform: null, textureUV: null);
            }

            foreach (Vector3 point in pointsOnShape)
            {
                PRCTransform transform = new PRCTransform();
                transform.XAxis = Intratech.Cores.Vector3.Xaxis;
                transform.YAxis = Intratech.Cores.Vector3.Yaxis;
                Intratech.Cores.Vector3 origin = new Intratech.Cores.Vector3(point.x, point.y, point.z);
                transform.Origin = origin;

                Intratech.PRC.Material3D color = Intratech.PRC.Material3D.Default;
                color.Diffuse = new Intratech.PRC.RGBAColor(_r: Color.Blue.R,
                                                            _g: Color.Blue.G,
                                                            _b: Color.Blue.B);
                color.Transparent = 1 - (float)currentItem.Geometry.ActiveTransparency;

                prcWriter.Geometry.AddSphere(name: "", radius: 0.001, material: color, transform: transform);
            }

            if (centerPoints != null)
            {
                foreach (Vector3 point in centerPoints)
                {
                    PRCTransform transform = new PRCTransform();
                    transform.XAxis = Intratech.Cores.Vector3.Xaxis;
                    transform.YAxis = Intratech.Cores.Vector3.Yaxis;
                    Intratech.Cores.Vector3 origin = new Intratech.Cores.Vector3(point.x, point.y, point.z);
                    transform.Origin = origin;

                    Intratech.PRC.Material3D color = Intratech.PRC.Material3D.Default;
                    color.Diffuse = new Intratech.PRC.RGBAColor(_r: Color.Green.R,
                                                                _g: Color.Green.G,
                                                                _b: Color.Green.B);
                    color.Transparent = 1 - (float)currentItem.Geometry.ActiveTransparency;

                    prcWriter.Geometry.AddSphere(name: "", radius: 0.001, material: color, transform: transform);
                }
            }
        }

        public static void SaveToPDF(Writer prcWriter)
        {
            //Save to pdf file
            string pdfOutput = Path.GetFullPath("test.pdf");
            string pdfTemplate = Path.GetFullPath("Landscape.pdf");
            string jsAnnot = "";//content of 3d annot javascript
            string jsDoc = "";//content of document javascript
            //Prepare pdf apperance settings: Toolbar, Model Tree, Background color, Rendering mode,....

            Intratech.PDF._3D.Appearance pdfSetting = new Intratech.PDF._3D.Appearance();
            pdfSetting.ShowModelTree = true;
            pdfSetting.ShowToolbar = true;
            pdfSetting.DefaultNavigationTool = NavigationTools.Spin;
            List<View> views = new List<View>();
            BoundingBox modelBox = new BoundingBox(new Vector3(), 10);
            AddDefaultViews(modelBox, ref views);
            PRCCreator.SavePDFWithExternalJS(prcWriter.SavePRC(), pdfOutput, views.ToArray(), pdfSetting, pdfTemplate, jsAnnot, jsDoc);
            Process.Start(pdfOutput);
        }

        private static void AddDefaultViews(BoundingBox bbox, ref List<View> views)
        {
            //Add default views
            List<ViewDirection> defaultViews = new List<ViewDirection>() { ViewDirection.SW, ViewDirection.TOP, ViewDirection.FRONT, ViewDirection.LEFT, ViewDirection.RIGHT, ViewDirection.BACK, ViewDirection.BOTTOM };
            foreach (var viewDirection in defaultViews)
            {
                var view = new View3D().AddView(bbox.min, bbox.max, viewDirection);
                view.Name = viewDirection.ToString();
                view.Lighting = Intratech.PDF.Graphics.Lighting.Headlamp;
                view.RenderMode = Intratech.PDF.Graphics.RenderMode.SolidOutline;
                views.Add(view);
            }
        }
    }
}
