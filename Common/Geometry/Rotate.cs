namespace Belok.Common.Geometry
{
    public class Transform4x4
    {
        public double[] translate_from;	//before rotation (-)
        public double[] rotate_to;		//direction of first rotation
        public double angle;			//angle of second rotation around direction rotate_to
        public double[] translate_to;	//after rotation (+) (or translate fixed object (-))
        public double[] angles;			//right rotates around X, Y, Z
        public double[] shifts;	
        public double[,] matrix;		//matrix of all transformations

	    public Transform4x4()
        {
            translate_from = new double[3];
            rotate_to = new double[3];
            translate_to = new double[3];
            angles = new double[3];
            shifts = new double[3];
            matrix = new double[4,4];
        }
	    //Transform4x4 operator = (Transform4x4 trn); //{angle = trn.angle; return *this;}
    };

    class Rotate
    {
        float FLT_EPSILON = 1.192092896e-07F;
        //void RotateMatrix(double fi_x, double fi_y, double fi_z, double rotate[3][3]);
        //Point_3 Rotate_x(Point_3 p, double s, double c);
        //Point_3 Rotate_y(Point_3 p, double s, double c);
        //Point_3 Rotate_z(Point_3 p, double s, double c);
        //Point_3 Rotate_fi(Point_3 na, Point_3 nb);
        //void Rotate_axis(Point_3 *p, Point_3 tna, double angle);
        public void WobbleMatrix(Point_3 tna, double angle, double[,] rotate)
        {
	        double c1,c2,c3,c,s,cc;
	        c1 = tna.x; c2 = tna.y; c3 = tna.z;
	        c = Math.Cos(angle); s = Math.Sin(angle); cc = (1.0 - c);
            //G.KORN T.KORN p.393 Russian edition Moscow 1970
	        rotate[0,0] = c + cc*c1*c1;
	        rotate[0,1] =     cc*c1*c2 - s*c3;
	        rotate[0,2] =     cc*c1*c3 + s*c2;
	        rotate[1,0] =     cc*c2*c1 + s*c3;
	        rotate[1,1] = c + cc*c2*c2;
	        rotate[1,2] =     cc*c2*c3 - s*c1;
	        rotate[2,0] =     cc*c3*c1 - s*c2;
	        rotate[2,1] =     cc*c3*c2 + s*c1;
	        rotate[2,2] = c + cc*c3*c3;
        }
        public void TransformPoint(Point_3 p, double[,] rotate)
        {
	        int i, j;
            double[] tmp = new double[3];
            double[]x = new double[3];
	        tmp[0] = tmp[1] = tmp[2] = 0.0;
	        x[0] = p.x; x[1] = p.y; x[2] = p.z;
	        for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += x[j] * rotate[i,j];
	        p.x = tmp[0]; p.y = tmp[1]; p.z = tmp[2];
        }
        private void Translate4x4 (Point_3 p, Transform4x4 res)
        {
	        int i,j;
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    if (i == j)
                        res.matrix[i, j] = 1;
                    else
                        res.matrix[i, j] = 0;
                }
            }
	        res.matrix[0,3] = p.x;
	        res.matrix[1,3] = p.y;
	        res.matrix[2,3] = p.z;
	        res.matrix[3,3] = 1;
        }
        private void Rotate4x4 (double[,] rotate, Transform4x4 res)
        {
	        int i,j;
	        for (i = 0; i < 3; i++) for (j = 0; j < 3; j++) res.matrix[i,j] = rotate[i,j];
	        for (i = 0; i < 3; i++) res.matrix[i,3] = 0;
	        for (j = 0; j < 3; j++) res.matrix[3,j] = 0;
	        res.matrix[3,3] = 1;
        }
        private void Multiply4x4 (Transform4x4 a, Transform4x4 b, Transform4x4 res)
        {
	        int i, j, k;
	        for (i = 0; i < 4; i++)
                for (j = 0; j < 4; j++)
                {
                    res.matrix[i, j] = 0;
                    for (k = 0; k < 4; k++)
                        res.matrix[i, j] += a.matrix[i, k] * b.matrix[k, j];
                }
		}
        private void Copy4x4 (Transform4x4 t, Transform4x4 res)
        {
	        int i,j;
	        for (i = 0; i < 4; i++) for (j = 0; j < 4; j++) res.matrix[i,j] = t.matrix[i,j];
        }
        public void TramsformTRRT (Point_3 p1, double[,] rotate1, double[,] rotate2, Point_3 p2, Transform4x4 res)
        {
	        Transform4x4 tmp0 = new Transform4x4();
            Transform4x4 tmp1 = new Transform4x4();
            Transform4x4 tmp2 = new Transform4x4();
            Transform4x4 tmp3 = new Transform4x4();
            Transform4x4 tmp4 = new Transform4x4();

	        p1.x = -p1.x; p1.y = -p1.y; p1.z = -p1.z;
            Translate4x4 (p1, tmp1);
            Rotate4x4 (rotate1, tmp2);
            Rotate4x4 (rotate2, tmp3);
            Translate4x4 (p2, tmp4);
            Multiply4x4 (tmp4, tmp3, tmp0); Copy4x4 (tmp0, tmp3);
            Multiply4x4 (tmp3, tmp2, tmp0); Copy4x4 (tmp0, tmp2);
            Multiply4x4 (tmp2, tmp1, tmp0); Copy4x4 (tmp0, res);
        }
        public void Transform(double[] x, Transform4x4 trn)
        {
	        int i, j;
            double[] tmp = new double[3];
	        tmp[0] = tmp[1] = tmp[2] = 0;
	        for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += x[j] * trn.matrix[i, j];
	        for(i = 0; i < 3; i++) x[i] = tmp[i] + trn.matrix[i, 3];
        }
        public double[] TransformTo(double[] x, Transform4x4 trn)
        {
            double[] y = new double[x.Length];
            int i, j;
            double[] tmp = new double[3];
            tmp[0] = tmp[1] = tmp[2] = 0;
            for (i = 0; i < 3; i++) for (j = 0; j < 3; j++) tmp[i] += x[j] * trn.matrix[i, j];
            for (i = 0; i < 3; i++) y[i] = tmp[i] + trn.matrix[i, 3];
            return y;
        }
        public void Transform(Point_3 p, Transform4x4 trn)
        {
            double[] x = new double[3] { p.x, p.y, p.z };
            Transform(x, trn);
            p.CopyFrom(x);
        }
        public Point_3[] Transform(Point_3[] ps, Transform4x4 trn)
        {
            Point_3[] pst = new Point_3[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                if(ps[i] != null)
                    pst[i] = TransformPTP(ps[i], trn);
            }
            return pst;
        }
        public Point_3 TransformPTP(Point_3 p, Transform4x4 trn)
        {
            Point_3 pt = new Point_3(p);
            double[] x = new double[3] { p.x, p.y, p.z };
            double[] y = TransformTo(x, trn);
            pt.CopyFrom(y);
            return pt;
        }
        public void construct(Transform4x4 trn)
        {
	        double[,] rotate = new double[3,3];
            double[,] rotate_wobble = new double[3,3];
            double norm;
            Point cen_a = new Point();
            Point cen_b = new Point();
            Point_3 tna = new Point_3();
	        cen_a.x = trn.translate_to[0];
	        cen_a.y = trn.translate_to[1];
	        cen_a.z = trn.translate_to[2];
	        cen_b.x = trn.translate_from[0];
	        cen_b.y = trn.translate_from[1];
	        cen_b.z = trn.translate_from[2];
        //	tna = p1a - p0a;
	        tna.x = trn.rotate_to[0] - trn.translate_to[0];
	        tna.y = trn.rotate_to[1] - trn.translate_to[1];
	        tna.z = trn.rotate_to[2] - trn.translate_to[2];
	        norm = ~tna;
	        if(norm > FLT_EPSILON) tna = tna / ~tna;
	        else { tna.x = 1; tna.y = 0; tna.z = 0;}
	        RotateMatrix(trn.angles[0], trn.angles[1], trn.angles[2], rotate);
	        WobbleMatrix(tna, (double)trn.angle, rotate_wobble);
	        TramsformTRRT(cen_b, rotate, rotate_wobble, cen_a, trn);
        }
        public void RotateMatrix(double fi_x, double fi_y, double fi_z, double[,] rotate)
        {
	        double s1,s2,s3,c1,c2,c3;
	        s1=Math.Sin(fi_x); s2=Math.Sin(fi_y); s3=Math.Sin(fi_z);
	        c1=Math.Cos(fi_x); c2=Math.Cos(fi_y); c3=Math.Cos(fi_z);
        //	rotate[0][0] = c2*c3;
        //	rotate[1][0] = s1*s2*c3-c1*s3;
        //	rotate[2][0] = c1*s2*c3+s1*s3;
        //	rotate[0][1] = c2*s3;
        //	rotate[1][1] = s1*s2*s3+c1*c3;
        //	rotate[2][1] = c1*s2*s3-s1*c3;
        //	rotate[0][2] = -s2;
        //	rotate[1][2] = s1*c2;
        //	rotate[2][2] = c1*c2;
	        rotate[0,0] =  (c2*c3);
	        rotate[0,1] =  (s1*s2*c3 - c1*s3);
	        rotate[0,2] =  (c1*s2*c3 + s1*s3);
	        rotate[1,0] =  (c2*s3);
	        rotate[1,1] =  (s1*s2*s3 + c1*c3);
	        rotate[1,2] =  (c1*s2*s3 - s1*c3);
	        rotate[2,0] =  (-s2);
	        rotate[2,1] =  (s1*c2);
	        rotate[2,2] =  (c1*c2);
        }
        public void EulerAnglers(out double fi_x, out double fi_y, out double fi_z, double[,] rotate)
        {
            double s1, s2, s3, c1, c2, c3;
            s2 = - rotate[2, 0];
            fi_y = Math.Asin(s2);
            c2 = Math.Cos(fi_y);
            s1 = rotate[2, 1] / c2;
            c1 = rotate[2, 2] / c2;
            fi_x = Math.Atan2(s1, c1);
            s3 = rotate[1, 0] / c2;
            c3 = rotate[0, 0] / c2;
            fi_z = Math.Atan2(s3, c3);
        }
        public double[,] GetRotate(Transform4x4 trn)
        {
            double[,] rotate = new double[3, 3];
            for(int i = 0; i < 3; i++)
                for(int j = 0; j < 3; j++)
                    rotate[i, j] = trn.matrix[i, j];
            return rotate;
        }
        void TramsformTRRT (Point p1, double[,] rotate1, double[,] rotate2, Point p2, Transform4x4 res)
        {
	        Transform4x4 tmp0 = new Transform4x4();
            Transform4x4 tmp1 = new Transform4x4();
            Transform4x4 tmp2 = new Transform4x4();
            Transform4x4 tmp3 = new Transform4x4();
            Transform4x4 tmp4 = new Transform4x4();
	        p1.x = -p1.x; p1.y = -p1.y; p1.z = -p1.z;
	        Translate4x4 (p1, tmp1);
	        Rotate4x4 (rotate1, tmp2);
	        Rotate4x4 (rotate2, tmp3);
	        Translate4x4 (p2, tmp4);
	        Multiply4x4 (tmp4, tmp3, tmp0); Copy4x4 (tmp0, tmp3);
	        Multiply4x4 (tmp3, tmp2, tmp0); Copy4x4 (tmp0, tmp2);
	        Multiply4x4 (tmp2, tmp1, tmp0); Copy4x4 (tmp0, res);
        }
        void Translate4x4 (Point p, Transform4x4 res)
        {
	        int i,j; 
	        for (i = 0; i < 4; i++) for (j = 0; j < 3; j++)
		        if(i == j) res.matrix[i,j] = 1; else res.matrix[i,j] = 0;
	        res.matrix[0,3] = p.x;
	        res.matrix[1,3] = p.y;
	        res.matrix[2,3] = p.z;
	        res.matrix[3,3] = 1;
        }
        //void Multiply4x4 (Transform4x4 a, Transform4x4 b, Transform4x4 *res);
        //void Translate4x4 (Point_3 p, double *res[4][4]);
        //void Translate4x4 (Point p, double *res[4][4]);
        //void Translate4x4 (Point p, Transform4x4 *res);
        //void Translate4x4 (Point_3 p, Transform4x4 *res);
        //void TramsformTRT (Point_3 p1, double rotate[3][3], Point_3 p2, Transform4x4 *res);
        //void TramsformTRRT (Point p1, double rotate1[3][3], double rotate2[3][3], Point p2, Transform4x4 *res);
        //void TramsformTRRT (Point_3 p1, double rotate1[3][3], double rotate2[3][3], Point_3 p2, Transform4x4 *res);
        //void Transform(double **pts, double **pts_trn, int n, Transform4x4 *trn);
        //void Transform(double **pts, int n, Transform4x4 *trn);
        //void Transform(double **pts, double **pts_trn, int n, double rotate[3][3]);
        //void Transform(double **pts, int i_start, int n, double rotate[3][3]);
        //void Transform_Rotate(double **pts, double **pts_trn, int n, Transform4x4 *trn);
        //void Transform_Rotate(double **pts, int i_start, int n, Transform4x4 *trn);
        //void Transform(double *x, Transform4x4 *trn);

        //void Transform(double *x, double rotate[3][3]);
        //void Copy4x4 (Transform4x4 t, Transform4x4 *res);
        //void Rotate4x4 (double rotate[3][3], Transform4x4 *res);
        //void construct(Transform4x4 *trn);
        //void construct(double *p, Transform4x4 *trn);
        //int trn_matrix(char* transfer, Transform4x4 *trn);
    }
}

/*
void Rotate :: RotateMatrix(double fi_x, double fi_y, double fi_z, double rotate[3][3])
{
	double s1,s2,s3,c1,c2,c3;
	s1=sin(fi_x); s2=sin(fi_y); s3=sin(fi_z);
	c1=cos(fi_x); c2=cos(fi_y); c3=cos(fi_z);
//	rotate[0][0] = c2*c3;
//	rotate[1][0] = s1*s2*c3-c1*s3;
//	rotate[2][0] = c1*s2*c3+s1*s3;
//	rotate[0][1] = c2*s3;
//	rotate[1][1] = s1*s2*s3+c1*c3;
//	rotate[2][1] = c1*s2*s3-s1*c3;
//	rotate[0][2] = -s2;
//	rotate[1][2] = s1*c2;
//	rotate[2][2] = c1*c2;

	rotate[0][0] =  double(c2*c3);
	rotate[0][1] =  double(s1*s2*c3 - c1*s3);
	rotate[0][2] =  double(c1*s2*c3 + s1*s3);
	rotate[1][0] =  double(c2*s3);
	rotate[1][1] =  double(s1*s2*s3 + c1*c3);
	rotate[1][2] =  double(c1*s2*s3 - s1*c3);
	rotate[2][0] =  double(-s2);
	rotate[2][1] =  double(s1*c2);
	rotate[2][2] =  double(c1*c2);
}

Point_3 Rotate :: Rotate_x(Point_3 p, double s, double c) {
	Point_3 res;
	res.x = p.x;
	res.y = double( p.y*c - p.z*s);
	res.z = double( p.y*s + p.z*c);
	return res;
}

Point_3 Rotate :: Rotate_y(Point_3 p, double s, double c) {
	Point_3 res;
	res.x = double( p.x*c + p.z*s);
	res.y = p.y;
	res.z = double(-p.x*s + p.z*c);
	return res;
}

Point_3 Rotate :: Rotate_z(Point_3 p, double s, double c) {
	Point_3 res;
	res.x = double( p.x*c - p.y*s);
	res.y = double( p.x*s + p.y*c);
	res.z = p.z;
	return res;
}

#define M_PI_2     1.57079632679489661923
Point_3 Rotate :: Rotate_fi(Point_3 na, Point_3 nb) {
	Point_3 tmp_x, tmp_y, tmp_z, res; double fi,fi1,fi2;
// x-rotate to nb.y = 0
	fi1 = atan2(nb.z, nb.y);								// angle to y
	fi2 = M_PI_2;																
	fi  = fi2 - fi1;										// angle to z	
	tmp_x = Rotate_x(nb, sin(fi), cos(fi));					// rotate to y = 0
	res.x = double(fi);
	if(fabs(nb.y) < 10.0*DBL_EPSILON) res.x = 0.0;
// y-rotate to nb.z = na.z							
	fi1 = atan2(sqrt(na.x*na.x+na.y*na.y),na.z);			// na angle to z
	fi2 = atan2(tmp_x.x,tmp_x.z);							// tmp_x angle to z
	fi  = fi1 - fi2;
	tmp_y = Rotate_y(tmp_x, sin(fi), cos(fi));				// rotate nb.z = na.z
	res.y = double(fi);
// z-rotate to coincidence nb to na
	fi1 = atan2(na.y, na.x);								// na angle to x
	fi2 = atan2(tmp_y.y, tmp_y.x);							// tmp_y angle to x
	fi  = fi1 - fi2;
	tmp_z = Rotate_z(tmp_y, sin(fi), cos(fi));				// rotate to coincidence
	res.z = double(fi);
	return res;
}	

void Rotate :: Rotate_axis(Point_3 *p, Point_3 tna, double angle)
{
	double rotate[3][3];
	WobbleMatrix(tna, angle, rotate);
	Transform(p, rotate);
}

void Rotate :: Multiply4x4 (Transform4x4 a, Transform4x4 b, Transform4x4 *res)
{
	int i, j, k;
	for (i = 0; i < 4; i++)
		for (j = 0; j < 4; j++) {
			res->matrix[i][j] = 0;
			for (k = 0; k < 4; k++)
				res->matrix[i][j] += a.matrix[i][k]*b.matrix[k][j];
		}
}

void Rotate :: Copy4x4 (Transform4x4 t, Transform4x4 *res)
{
	int i,j;
	for (i = 0; i < 4; i++) for (j = 0; j < 4; j++) res->matrix[i][j] = t.matrix[i][j];
}

void Rotate :: Rotate4x4 (double rotate[3][3], Transform4x4 *res)
{
	int i,j;
	for (i = 0; i < 3; i++) for (j = 0; j < 3; j++) res->matrix[i][j] = rotate[i][j];
	for (i = 0; i < 3; i++) res->matrix[i][3] = 0;
	for (j = 0; j < 3; j++) res->matrix[3][j] = 0;
	res->matrix[3][3] = 1;
}

void Rotate :: Translate4x4 (Point_3 p, Transform4x4 *res)
{
	int i,j;
	for (i = 0; i < 4; i++) for (j = 0; j < 3; j++) 
		if(i == j) res->matrix[i][j] = 1; else res->matrix[i][j] = 0;
	res->matrix[0][3] = p.x;
	res->matrix[1][3] = p.y;
	res->matrix[2][3] = p.z;
	res->matrix[3][3] = 1;
}

void Rotate :: Translate4x4 (Point p, Transform4x4 *res)
{
	int i,j; 
	for (i = 0; i < 4; i++) for (j = 0; j < 3; j++)
		if(i == j) res->matrix[i][j] = 1; else res->matrix[i][j] = 0;
	res->matrix[0][3] = p.x;
	res->matrix[1][3] = p.y;
	res->matrix[2][3] = p.z;
	res->matrix[3][3] = 1;
}

void Rotate :: TramsformTRRT (Point p1, double rotate1[3][3], double rotate2[3][3], Point p2, Transform4x4 *res)
{
	Transform4x4 tmp0, tmp1, tmp2, tmp3, tmp4;
	p1.x = -p1.x; p1.y = -p1.y; p1.z = -p1.z;
	Translate4x4 (p1,	&tmp1);
	Rotate4x4 (rotate1, &tmp2);
	Rotate4x4 (rotate2, &tmp3);
	Translate4x4 (p2,	&tmp4);
	Multiply4x4 (tmp4, tmp3, &tmp0); Copy4x4 (tmp0, &tmp3);
	Multiply4x4 (tmp3, tmp2, &tmp0); Copy4x4 (tmp0, &tmp2);
	Multiply4x4 (tmp2, tmp1, &tmp0); Copy4x4 (tmp0, res);
}

void Rotate :: TramsformTRRT (Point_3 p1, double rotate1[3][3], double rotate2[3][3], Point_3 p2, Transform4x4 *res)
{
	Transform4x4 tmp0, tmp1, tmp2, tmp3, tmp4;
	p1.x = -p1.x; p1.y = -p1.y; p1.z = -p1.z;
	Translate4x4 (p1,	&tmp1);
	Rotate4x4 (rotate1, &tmp2);
	Rotate4x4 (rotate2, &tmp3);
	Translate4x4 (p2,	&tmp4);
	Multiply4x4 (tmp4, tmp3, &tmp0); Copy4x4 (tmp0, &tmp3);
	Multiply4x4 (tmp3, tmp2, &tmp0); Copy4x4 (tmp0, &tmp2);
	Multiply4x4 (tmp2, tmp1, &tmp0); Copy4x4 (tmp0, res);
}

void Rotate :: TramsformTRT (Point_3 p1, double rotate[3][3], Point_3 p2, Transform4x4 *res)
{
	Transform4x4 tmp0, tmp1, tmp2, tmp3, tmp4;
	p1.x = -p1.x; p1.y = -p1.y; p1.z = -p1.z;
	Translate4x4 (p1,	&tmp1);
	Rotate4x4 (rotate, &tmp2);
	//Rotate4x4 (rotate2, &tmp3);
	Translate4x4 (p2,	&tmp4);
	//Multiply4x4 (tmp4, tmp3, &tmp0); Copy4x4 (tmp0, &tmp3);
	//Multiply4x4 (tmp3, tmp2, &tmp0); Copy4x4 (tmp0, &tmp2);
	Multiply4x4 (tmp4, tmp2, &tmp0); Copy4x4 (tmp0, &tmp2);
	Multiply4x4 (tmp2, tmp1, &tmp0); Copy4x4 (tmp0, res);
}

void Rotate :: Transform(double **pts, double **pts_trn, int n, Transform4x4 *trn) //pts[3][n]
{
	int i, j, k; double tmp[3];
	for(k = 0; k < n; k++){
		tmp[0] = tmp[1] = tmp[2] = 0;
		for(i = 0; i < 3; i++) {
			for(j = 0; j < 3; j++) {
				tmp[i] += pts[j][k] * trn->matrix[i][j];
			}
		}
		for(i = 0; i < 3; i++) pts_trn[i][k] = tmp[i] + trn->matrix[i][3];
	}
}

void Rotate :: Transform(double **pts, int n, Transform4x4 *trn) //pts[3][n]
{
	int i, j, k; double tmp[3];
	for(k = 0; k < n; k++){
		tmp[0] = tmp[1] = tmp[2] = 0;
		for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += pts[j][k] * trn->matrix[i][j];
		for(i = 0; i < 3; i++) pts[i][k] = tmp[i] + trn->matrix[i][3];
	}
}

void Rotate :: Transform(double **pts, double **pts_trn, int n, double rotate[3][3])
{
	int i, j, k; double tmp[3];
	for(k = 0; k < n; k++){
		tmp[0] = tmp[1] = tmp[2] = 0;
		for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += pts[j][k] * rotate[i][j];
		for(i = 0; i < 3; i++) pts_trn[i][k] = tmp[i];
	}
}

void Rotate :: Transform(double **pts, int i_start, int n, double rotate[3][3])
{
	int i, j, k; double tmp[3];
	for(k = i_start; k < i_start + n; k++){
		tmp[0] = tmp[1] = tmp[2] = 0;
		for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += pts[j][k] * rotate[i][j];
		for(i = 0; i < 3; i++) pts[i][k] = tmp[i];
	}
}

void Rotate :: Transform(double *x, double rotate[3][3])
{
	int i, j; double tmp[3];
	tmp[0] = tmp[1] = tmp[2] = 0;
	for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += x[j] * rotate[i][j];
	for(i = 0; i < 3; i++) x[i] = tmp[i];
}

void Rotate :: Transform(double *x, Transform4x4 *trn)
{
	int i, j; double tmp[3];
	tmp[0] = tmp[1] = tmp[2] = 0;
	for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += x[j] * trn->matrix[i][j];
	for(i = 0; i < 3; i++) x[i] = tmp[i] + trn->matrix[i][3];
}

void Rotate :: Transform_Rotate(double **pts, double **pts_trn, int n, Transform4x4 *trn) //pts[3][n]
{
	int i, j, k; double tmp[3];
	for(k = 0; k < n; k++){
		tmp[0] = tmp[1] = tmp[2] = 0;
		for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += pts[j][k] * trn->matrix[i][j];
		for(i = 0; i < 3; i++) pts_trn[i][k] = tmp[i];
	}
}

void Rotate :: Transform_Rotate(double **pts, int i_start, int n, Transform4x4 *trn)
{
	int i, j, k; double tmp[3];
	for(k = i_start; k < i_start + n; k++){
		tmp[0] = tmp[1] = tmp[2] = 0;
		for(i = 0; i < 3; i++) for(j = 0; j < 3; j++) tmp[i] += pts[j][k] * trn->matrix[i][j];
		for(i = 0; i < 3; i++) pts[i][k] = tmp[i];
	}
}

void Rotate :: construct(Transform4x4 *trn)
{
	double rotate[3][3], norm, rotate_wobble[3][3]; Point cen_a, cen_b; Point_3 tna;
	cen_a.x = trn->translate_to[0];
	cen_a.y = trn->translate_to[1];
	cen_a.z = trn->translate_to[2];
	cen_b.x = trn->translate_from[0];
	cen_b.y = trn->translate_from[1];
	cen_b.z = trn->translate_from[2];
//	tna = p1a - p0a;
	tna.x = trn->rotate_to[0] - trn->translate_to[0];
	tna.y = trn->rotate_to[1] - trn->translate_to[1];
	tna.z = trn->rotate_to[2] - trn->translate_to[2];
	norm = ~tna;
	if(norm > FLT_EPSILON) tna = tna / ~tna;
	else { tna.x = 1; tna.y = 0; tna.z = 0;}
	RotateMatrix(trn->angles[0], trn->angles[1], trn->angles[2], rotate);
	WobbleMatrix(tna, double(trn->angle), rotate_wobble);
	TramsformTRRT(cen_b, rotate, rotate_wobble, cen_a, trn);
} 

void Rotate :: construct(double *p, Transform4x4 *trn)
{
	double rotate[3][3], rotate_wobble[3][3]; Point cen_a, cen_b; Point_3 tna;
	cen_a.x = trn->translate_to[0] + p[1];
	cen_a.y = trn->translate_to[1] + p[2];
	cen_a.z = trn->translate_to[2] + p[3];
	cen_b.x = trn->translate_from[0];
	cen_b.y = trn->translate_from[1];
	cen_b.z = trn->translate_from[2];
//	tna = p1a - p0a;
	tna.x = trn->rotate_to[0] - trn->translate_to[0];
	tna.y = trn->rotate_to[1] - trn->translate_to[1];
	tna.z = trn->rotate_to[2] - trn->translate_to[2];
	tna = tna / ~tna;
	RotateMatrix(p[4], p[5], p[6], rotate);
	WobbleMatrix(tna, trn->angle, rotate_wobble);
	TramsformTRRT(cen_b, rotate, rotate_wobble, cen_a, trn);
}

int Rotate :: trn_matrix(char* transfer, Transform4x4 *trn)
{
	char seps[] = "$"; char *token; int i, j;
	for(i = 0; i < 3; i++) for(j = 0; j < 4; j++) {
		if(i == 0 && j == 0) token = strtok(transfer, seps);
		else token = strtok(NULL, seps);
		if(token != NULL) trn->matrix[i][j] = double(atof(token));
		else return false;
	}
	for(i = 0; i < 3; i++) trn->matrix[3][i] = 0.0;
	trn->matrix[3][3] = 1.0;
	return true;
}
*/