using Intratech.Cores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intra.GeometryDetection
{
    public class DataGeometry
    {
        public List<Vector3> Vertices { get; set; }
        public List<int> Faces { get; set; }

        public DataGeometry(List<Vector3> vertices, List<int> faces)
        {
            Vertices = vertices;
            Faces = faces;
        }

        public List<LineItem> createLinesFromTriangles()
        {
            Dictionary<int, List<int>> lineDictionary = Enumerable.Range(0, Vertices.Count).ToDictionary(x => x, x => new List<int>());

            List<LineItem> lineItems = new List<LineItem>();
            for(int i = 0; i < Faces.Count; i++)
            {
                if (i % 3 == 0)
                {
                    if (!lineDictionary[Faces[i]].Contains(Faces[i + 1]))
                    {
                        lineItems.Add(new LineItem(Vertices[Faces[i]], Vertices[Faces[i + 1]]));
                        lineDictionary[Faces[i]].Add(Faces[i + 1]);
                        lineDictionary[Faces[i + 1]].Add(Faces[i]);
                    }

                    if (!lineDictionary[Faces[i]].Contains(Faces[i + 2]))
                    {
                        lineItems.Add(new LineItem(Vertices[Faces[i]], Vertices[Faces[i + 2]]));
                        lineDictionary[Faces[i]].Add(Faces[i + 2]);
                        lineDictionary[Faces[i + 2]].Add(Faces[i]);
                    }

                    if (!lineDictionary[Faces[i + 1]].Contains(Faces[i + 2]))
                    {
                        lineItems.Add(new LineItem(Vertices[Faces[i + 1]], Vertices[Faces[i + 2]]));
                        lineDictionary[Faces[i + 1]].Add(Faces[i + 2]);
                        lineDictionary[Faces[i + 2]].Add(Faces[i + 1]);
                    }
                }  
            }

            return lineItems;
        }
    }
}
