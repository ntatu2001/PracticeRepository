using Intratech.Cores;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;

namespace MemberDetection
{
    public enum MemberType
    {
        squareBar,
        roundBar,
        angleType,
        otherMemberStructure
    }

    public class DetectType
    {
        public List<Vector2> PointsOnHull { get; set; }

        public DetectType(List<Vector2> pointsOnHull)
        {
            this.PointsOnHull = pointsOnHull;
        }

        public MemberType detect()
        {
            //Create list of lines between points removed on the Convex Hull.
            ListLines listLineItems = new ListLines(this.PointsOnHull);

            ListLines removedListLineItems = listLineItems.removedLineParallel();
            List<double> angles = removedListLineItems.listAngleBetweenLines();

            ListPoints listPoints = new ListPoints(point2Ds: PointsOnHull);
            Dictionary<Vector2, double> point2DAngleDictionary = listPoints.removedPointOnParallelLines(angles);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\AnhTu\\Member Structure Detection\\MemberDetection\\MemberDetection\\PointOnHullRemoved.txt"))
            {
                foreach (Vector2 point in point2DAngleDictionary.Keys)
                {
                    writer.WriteLine($"({point.x},{point.y})");
                }
            }


            //Given a dictionary having the max number of angles equal, the value is the angle, the count is the number of angles in group.
            var maxAngleEqual = angles.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() })
                                      .OrderByDescending(x => x.Count).FirstOrDefault();

            int numberOfAngleEqual = angles.Where(x => x >= maxAngleEqual.Value * (1 - 0.02) && x <= maxAngleEqual.Value * (1 + 0.02)).ToList().Count;
            int numberObtuseAngle = angles.Where(x => x < 60).ToList().Count;

            if (angles.Count == 4 && maxAngleEqual.Value >= 90*(1 - 0.02) && maxAngleEqual.Value <= 90 * (1 + 0.02) && numberOfAngleEqual == 4)
            {
                //Selected item just has 4 points on Hull and angles are equal 90 degree.
                Vector3 firstVector = removedListLineItems.Lines[0].vector;

                //Compare the length of each vector. If the lengths of 4 vector are equal, it is the square bar.
                var notSameLength = removedListLineItems.Lines.Where(x => x.vector.Length <= firstVector.Length * (1 - 0.2) || x.vector.Length >= firstVector.Length * (1 + 0.2)).ToList();

                if (notSameLength.Count == 0)
                    return MemberType.squareBar;
            }
            else if (numberObtuseAngle >= 3)
            {
                //If they have more than 3 obtuse angle on Convex Hull, it is the round bar.
                return MemberType.roundBar;
            }
            else if (angles.Count == 5)
            {
                //If they just have 5 points on Convex Hull and contains 3 square angles in list of angles, it is the angle type.
                int numberAngleEqual = angles.Where(x => x >= 90 * (1 - 0.1) && x <= 90 * (1 + 0.1)).ToList().Count;

                if (numberAngleEqual < 4)
                    return MemberType.angleType;
            }

            return MemberType.otherMemberStructure;
        }
    }
}
