using Intratech.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Intra.GeometryDetection
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
            List<double> listAngleDict = point2DAngleDictionary.Values.ToList();
            List<Vector2> point2DsRemoved = point2DAngleDictionary.Keys.ToList();

            //Given a dictionary having the max number of angles equal, the value is the angle, the count is the number of angles in group.
            var maxAngleEqual = angles.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() })
                                      .OrderByDescending(x => x.Count).FirstOrDefault();

            int numberOfAngleEqual = listAngleDict.Where(x => x >= maxAngleEqual.Value * (1 - 0.05) && x <= maxAngleEqual.Value * (1 + 0.05)).ToList().Count;
            int numberObtuseAngle = listAngleDict.Where(x => x < 60).ToList().Count;

            if (listAngleDict.Count == 4 && maxAngleEqual.Value >= 90 * (1 - 0.05) && maxAngleEqual.Value <= 90 * (1 + 0.05) && numberOfAngleEqual == 4)
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
            else if (listAngleDict.Count == 5)
            {
                //If they just have 5 points on Convex Hull and contains 3 square angles in list of angles, it is the angle type.
                int numberAngleEqual = listAngleDict.Where(x => x >= 90 * (1 - 0.1) && x <= 90 * (1 + 0.1)).ToList().Count;

                if (numberAngleEqual < 4)
                    return MemberType.angleType;
            }

            return MemberType.otherMemberStructure;
        }
    }
}
