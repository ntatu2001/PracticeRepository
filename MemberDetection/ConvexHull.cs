// C# equivalent of the above code
using Intratech.Cores;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MemberDetection
{
    public class ConvexHul
    {
        // Define Infinite (Using INT_MAX  caused overflow problems)
        const int INF = 10000;
        public Vector2[] Points { get; set; }

        public ConvexHul(Vector2[] points)
        {
            Points = points;
        }


        // To find orientation of ordered triplet (p, q, r). The function returns following values 0 --> p, q and r are colinear  1 --> Clockwise  2 --> Counterclockwise
        //static int Orientation(Vector2 p, Vector2 q, Vector2 r)
        //{
        //    // See https://www.geeksforgeeks.org/orientation-3-ordered-points/  for details of below formula.
        //    double val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        //    if (val == 0)
        //        return 0;  // colinear

        //    return (val > 0) ? 1 : 2; // clock or counterclock wise
        //}

        //// Prints convex hull of a set of n points.
        //public List<Vector2> convexHull()
        //{
        //    int n = this.Points.Length;

        //    // There must be at least 3 points
        //    if (n < 3)
        //        return null;

        //    // Initialize Result
        //    List<Vector2> hull = new List<Vector2>();

        //    // Find the leftmost point
        //    int l = 0;
        //    for (int i = 1; i < n; i++)
        //    {
        //        if (this.Points[i].x < this.Points[l].x)
        //            l = i;
        //    }

        //    // Start from leftmost point, keep moving  counterclockwise until reach the start point  again. This loop runs O(h) times where h is number of points in result or output.
        //    int p = l, q;
        //    int iteration = n;
        //    do
        //    {
        //        // Add current point to result
        //        hull.Add(this.Points[p]);

        //        // Search for a point 'q' such that  orientation(p, x, q) is counterclockwise  for all points 'x'. The idea is to keep  track of last visited most counterclock- wise point in q. If any point 'i' is more counterclockwise than q, then update q.
        //        q = (p + 1) % n;

        //        for (int i = 0; i < n; i++)
        //        {
        //            // If i is more counterclockwise than  current q, then update q
        //            if (Orientation(this.Points[p], this.Points[i], this.Points[q]) == 2)
        //                q = i;
        //        }

        //        // Now q is the most counterclockwise with respect to p. Set p as q for next iteration, so that q is added to result 'hull'
        //        p = q;
        //        iteration--;
        //    } while (p != l || iteration == 0);  // While we don't come to first 
        //                                        // point

        //    // Print Result

        //    return hull;
        //}

        public static double cross(Vector2 O, Vector2 A, Vector2 B)
        {
            return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
        }

        public List<Vector2> GetConvexHull(List<Vector2> points)
        {
            if (points == null)
                return null;

            if (points.Count() <= 1)
                return points;

            int n = points.Count(), k = 0;
            List<Vector2> H = new List<Vector2>(new Vector2[2 * n]);

            points.Sort((a, b) =>
                 a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

            // Build lower hull
            for (int i = 0; i < n; ++i)
            {
                while (k >= 2 && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                    k--;
                H[k++] = points[i];
            }

            // Build upper hull
            for (int i = n - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                    k--;
                H[k++] = points[i];
            }

            return H.Take(k - 1).ToList();
        }
    }
}

