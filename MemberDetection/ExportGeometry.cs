using Autodesk.Navisworks.Api;
using System.IO;
using System.Windows.Forms;

namespace MemberDetection
{
    public class ExportGeometry
    {
        public ModelItemCollection modelItems { get; set; }

        public ExportGeometry(ModelItemCollection modelItems)
        {
            this.modelItems = modelItems;
        }

        public void exportGeometry(string filePath)
        {
            FileStream aFile = new FileStream(filePath, FileMode.Truncate);
            DataGeometry dataGeometry = new DataGeometry();
            dataGeometry = dataGeometry.getGeometry(this.modelItems);

            using (StreamWriter writer = new StreamWriter(aFile))
            {
                foreach (VertexFragment vertex in dataGeometry.Vertices)
                {
                    writer.WriteLine($"v {vertex.VertexX} {vertex.VertexY} {vertex.VertexZ}");
                }

                foreach (int[] face in dataGeometry.Faces)
                {
                    writer.WriteLine($"f {face[0] + 1} {face[1] + 1} {face[2] + 1}");
                }

                writer.Close();
            }

            MessageBox.Show("Export geometry successfully");
            aFile.Close();
        }

        
    }
}
