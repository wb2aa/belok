namespace Belok.Common.Geometry
{
    public class dTriangle
    {
        public Point_3[] p = new Point_3[3];
        public Point_3[] n = new Point_3[3];
        public Point_3 norm = new Point_3();
        public int[] pointindex = new int[3];
        public dTriangle(Point_3[] P, Point_3[] N, int[] Pointindex)
        {
            p = P;
            n = N;
            for (int i = 0; i < 3; i++)
                pointindex[i] = Pointindex[i];
        }
        public dTriangle(Point_3 p1, Point_3 p2, Point_3 p3)
        {
            p[0] = p1;
            p[1] = p2;
            p[2] = p3;
        }
        public void CalculateNormal()
        {
            norm.x = p[0].y * (p[1].z - p[2].z) - p[1].y * (p[0].z - p[2].z) + p[2].y * (p[0].z - p[1].z);
            norm.y = p[0].z * (p[1].x - p[2].x) - p[1].z * (p[0].x - p[2].x) + p[2].z * (p[0].x - p[1].x);
            norm.z = p[0].x * (p[1].y - p[2].y) - p[1].x * (p[0].y - p[2].y) + p[2].x * (p[0].y - p[1].y);
            norm = norm / ~norm;
        }
    }
}