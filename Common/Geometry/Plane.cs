namespace Belok.Common.Geometry
{
    class Plane
    {
        //n.x*x + n.y*y *n.z*z - D = 0; D >=0  canonical plane equation
        public Point_3 n; //normal
        public double D;  //distance to (0,0,0) origin D >= 0

        public Plane()
        {
            n = new Point_3(1.0, 0.0, 0.0);
            D = 0;
        }

        public Plane(Plane plane)
        {
            n = new Point_3(plane.n);
            D = plane.D;
        }

        //bool Set_Plane(double[,] pts, int i0);
        public bool Set_Plane(Point_3 p1, Point_3 p2, Point_3 p3)
        {
            double POINTS_INLINE = 0.999;
	        double norm; Point_3 dp1, dp2; double c;
	        dp1 = p2 - p1; dp1 = dp1 / ~dp1;
	        dp2 = p3 - p2; dp2 = dp2 / ~dp2;
	        c = Math.Abs(dp1 * dp2);
	        n.x = p1.y * (p2.z - p3.z) - p2.y * (p1.z - p3.z) + p3.y * (p1.z - p2.z);
	        n.y = p1.z * (p2.x - p3.x) - p2.z * (p1.x - p3.x) + p3.z * (p1.x - p2.x);
	        n.z = p1.x * (p2.y - p3.y) - p2.x * (p1.y - p3.y) + p3.x * (p1.y - p2.y);
	        D =   p1.x * (p2.y*p3.z - p3.y*p2.z) - p2.x * (p1.y*p3.z - p3.y*p1.z) + p3.x * (p1.y*p2.z - p2.y*p1.z);
	        //if(fabs(D) > DBL_EPSILON) {
	        if(c < POINTS_INLINE) {
		        norm = ~n; if(D < 0.0) norm = - norm;
		        n = n / norm; D /= norm;
		        return true;
	        }
	        else {
		        n.x = 0; n.y = 0; n.z = 1.0;
		        D = 0;
		        return false;
	        }
        }
        public Point_3 Get_Normal(Point_3 p1, Point_3 p2, Point_3 p3)
        {
            Point_3 n = new Point_3();
            n.x = p1.y * (p2.z - p3.z) - p2.y * (p1.z - p3.z) + p3.y * (p1.z - p2.z);
            n.y = p1.z * (p2.x - p3.x) - p2.z * (p1.x - p3.x) + p3.z * (p1.x - p2.x);
            n.z = p1.x * (p2.y - p3.y) - p2.x * (p1.y - p3.y) + p3.x * (p1.y - p2.y);
            n = n / ~n;
            return n;
        }
        public bool Set_Plane_normal(Point_3 p1, Point_3 p0, Point_3 p2)
        {
            bool res; Point_3 p_tmp, n_tmp;
            Plane pln = new Plane();
            res = pln.Set_Plane(p1, p0, p2);
            if (res)
            {
                p_tmp = (p1 + p2) / 2.0;
                n_tmp = pln.n;
                res = pln.Set_Plane(p0, p_tmp, p0 + n_tmp);
                if (res)
                {
                    n = pln.n;
                    D = pln.D;
                }
                else
                {
                    n.x = 0; n.y = 0; n.z = 1.0;
                    D = 0;
                }
            }
            else
            {
                n.x = 0; n.y = 0; n.z = 1.0;
                D = 0;
            }
            return res;
        }
        public bool Set_Plane_normal_p(Point_3 p1, Point_3 p2, Point_3 p0)
        {
            bool res; Point_3 n_tmp;
            Plane pln = new Plane(); 
            res = pln.Set_Plane(p1, p2, p0);
            if (res)
            {
                n_tmp = pln.n;
                res = pln.Set_Plane(p1, p2, p1 + n_tmp);
                if (res)
                {
                    n = pln.n;
                    D = pln.D;
                }
                else
                {
                    n.x = 0; n.y = 0; n.z = 1.0;
                    D = 0;
                }
            }
            else
            {
                n.x = 0; n.y = 0; n.z = 1.0;
                D = 0;
            }
            return res;
        }
        public double dist(Point_3 p)
        {
            return n*p - D;
        }
        //Point_3 Intersection(Point_3 x, Point_3 v);
    }
}
/*
bool Plane :: Set_Plane(double **pts, int i0)
{
	Point_3 p1, p2, p3; bool ret;
	p1.Set(pts, i0);
	p2.Set(pts, i0 + 1);
	p3.Set(pts, i0 + 2);
	ret = Set_Plane(p1, p2, p3);
	return ret;
}




double Plane :: dist(Point_3 p)
{
	return n*p - D;
}



bool Plane :: Set_Plane_normal_p(Point_3 p1, Point_3 p2, Point_3 p0)
{

}

Point_3 Plane :: Intersection(Point_3 x, Point_3 v)
{
	Point_3 res; double t;
	t = (D - n * x) / (n * v);
	res = x + v * t;
	return res;
}
    }
}
*/