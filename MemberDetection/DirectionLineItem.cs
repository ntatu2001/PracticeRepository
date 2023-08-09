using Intratech.Cores;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace MemberDetection
{
    public class DirectionLineItem
    {
        public List<LineItem> LineItems { get; set; }

        public DirectionLineItem(List<LineItem> lineItems)
        {
            LineItems = lineItems;
        }

        public LineItem getDirectionLineItem()
        {
            List<LineItem> chosenParallelVectorGroup = this.getDirectionLineGroup(this.LineItems);

            // Get the line which have the max length of vector
            LineItem directionVector = chosenParallelVectorGroup.OrderByDescending(x => x.vector.Length).FirstOrDefault();
            return directionVector;
        }

        public List<LineItem> getDirectionLineGroup(List<LineItem> lineItems)
        {
            //Given group of parallel lines
            Dictionary<LineItem, List<LineItem>> parallelLineDictionary = new Dictionary<LineItem, List<LineItem>> { { lineItems[0], new List<LineItem>() { lineItems[0] } } };
            for (int i = 1; i < lineItems.Count; i++)
            {
                bool isExistParallel = false;
                foreach (LineItem lineKey in parallelLineDictionary.Keys)
                {
                    if (Vector3.Angle(lineKey.vector, lineItems[i].vector) <= 0.001 || Vector3.Angle(lineKey.vector, lineItems[i].vector) >= 179.999)
                    {
                        isExistParallel = true;
                        parallelLineDictionary[lineKey].Add(lineItems[i]);
                    }
                }

                if (!isExistParallel)
                    parallelLineDictionary.Add(lineItems[i], new List<LineItem>() { lineItems[i] });
            }

            //Filter groups which have the number of lines higher than 3, create a new list of List lines sorted by the count.
            List<List<LineItem>> group = parallelLineDictionary.Where(x => x.Value.Count > 3).Select(y => y.Value).OrderByDescending(x => x.Count).ToList();

            //Create a new List of list of each vector's length in each group.
            List<List<float>> lengthOfEachGroup = group.Select(x => x.Select(y => y.vector.Length).ToList()).ToList();

            //Create a new list that contains max length of vector in each group.
            List<float> maxVectorLengthList = group.Select(x => x.Max(y => y.vector.Length)).ToList();
            float maxLength = maxVectorLengthList.OrderByDescending(x => x).First();

            //Choose the group of line which has max length of vector.
            List<LineItem> chosenParallelLineGroup = group[maxVectorLengthList.IndexOf(maxLength)];

            return chosenParallelLineGroup;
        }
    }
}
