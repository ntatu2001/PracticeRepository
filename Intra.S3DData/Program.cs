using Intratech.Cores;
using Intratech.Cores.Geometries;
using System;
using System.Diagnostics;
using System.IO;

namespace Intra.GeometryDetection
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            foreach (var file in Directory.GetFiles("C:\\Users\\AnhTu\\Member Structure Detection\\SN2592_FM402_Members_Obj"))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Mesh mesh = new Mesh();
                mesh.ReadObj(file);
                MemberPartDetector memberPartDetector = new MemberPartDetector();
                var result = memberPartDetector.detect(mesh, out MemberType memberType, out Vector3? firstCenter, out Vector3? secondCenter, out double? maxDistance);

                Console.WriteLine($"The type of selected item is {memberType.ToString()}");
                if (memberType == MemberType.squareBar || memberType == MemberType.roundBar)
                {
                    Console.WriteLine($"The center points of shape are:\n({firstCenter.Value.x}, {firstCenter.Value.y}, {firstCenter.Value.z})\n({secondCenter.Value.x}, {secondCenter.Value.y}, {secondCenter.Value.z})");
                }
                else if (memberType == MemberType.angleType)
                {
                    Console.WriteLine($"The center points of shape are:\n({firstCenter.Value.x}, {firstCenter.Value.y}, {firstCenter.Value.z})\n({secondCenter.Value.x}, {secondCenter.Value.y}, {secondCenter.Value.z})\nThe max length of shape is: {(firstCenter - secondCenter)?.Length}");
                }
                sw.Stop();
                Console.WriteLine($"Process file: {file} in {sw.ElapsedMilliseconds.ToString()} miliseconds");
            }

        }
    }
}
