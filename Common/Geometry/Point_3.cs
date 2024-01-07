namespace Belok.Common.Geometry
{
    public class Point
    {
        public double x, y, z;
        public Point() { x = 0; y = 0; z = 0; }
        //Point operator=(Point p) {x=p.x; y=p.y; z=p.z; return *this;}
    }
    public class Point_3
    {
        public double x, y, z;
        public Point_3() { x = 0; y = 0; z = 0; }
        public Point_3(double dx, double dy, double dz) { x = dx; y = dy; z = dz; }
        public Point_3(double[] v) { x = v[0]; y = v[1]; z = v[2]; }
        public Point_3(Point_3 p) { x = p.x; y = p.y; z = p.z; }
        public static double operator ~(Point_3 p) { return Math.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z); }
        public static Point_3 operator /(Point_3 p, double norm) { return new Point_3(p.x / norm, p.y / norm, p.z / norm); }
        public static Point_3 operator *(Point_3 p, double scale) { return new Point_3(p.x * scale, p.y * scale, p.z * scale); }
        public static double operator *(Point_3 p1, Point_3 p2) { return (p1.x * p2.x + p1.y * p2.y + p1.z * p2.z); }
        public static Point_3 operator ^(Point_3 p1, Point_3 p2) { return new Point_3(p1.y * p2.z - p1.z * p2.y, -p1.x * p2.z + p1.z * p2.x, p1.x * p2.y - p1.y * p2.x); }
        public static Point_3 operator +(Point_3 p1, Point_3 p2) { return new Point_3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z); }
        public static Point_3 operator -(Point_3 p1, Point_3 p2) { return new Point_3(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z); }
        public void CopyFrom(Point_3 p) { x = p.x; y = p.y; z = p.z; }
        public void CopyTo(double[] v) { v[0] = x; v[1] = y; v[2] = z; }
        public void CopyFrom(double[] v) { x = v[0]; y = v[1]; z = v[2]; }
        public double Distance(Point_3 p) { Point_3 tmp = new Point_3(p.x - x, p.y - y, p.z - z); return ~tmp; }
    }
}
/*
	public bool flag;
//???
	Point_3 operator=(Point_3 p) {x=p.x; y=p.y; z=p.z; flag=p.flag; return *this;}
//???
	//Point_3 operator+() {x=x; y=y; z=z; return *this;}
	Point_3 operator+() {return Point_3(x, y, z);}
	//Point_3 operator-() {x=-x; y=-y; z=-z; return *this;}
	Point_3 operator-() {return Point_3(-x, -y, -z);}
	//Point_3 operator/(double norm) {x=x/norm; y=y/norm; z=z/norm; return *this;}
	
	//Point_3 operator*(double norm) {x=x*norm; y=y*norm; z=z*norm; return *this;}
	Point_3 operator*(double norm) {return Point_3(x*norm, y*norm, z*norm);}
	Point_3 operator+(Point_3 p) {return Point_3(x+p.x, y+p.y, z+p.z);}
	Point_3 operator-(Point_3 p) {return Point_3(x-p.x, y-p.y, z-p.z);}
	double operator*(Point_3 p) {return (x*p.x + y*p.y + z*p.z);}

	double operator!() {return (x*x+y*y+z*z);}

	void TransformPoint (Transform4x4 trn);
	void Set(double **pts, int i);
	void Set(int i, double d);
	void Set(double *v);
	void CopyTo(double **pts, int i);
	void CopyTo(double *v);
	Point_3 Get_Normal(Point_3 v);

	float distance(Point_3 *p) {
		Point_3 tmp(p->x - x, p->y - y, p->z - z);
		return float(~tmp);
	}

	double distance(Point_3 p) {
		Point_3 tmp(p.x - x, p.y - y, p.z - z);
		return float(~tmp);
	}

	float dot(Point_3 *p) {
		return float(p->x*x + p->y*y + p->z*z);
	}

	Point_3 add_to_cluster(Point_3 *b, float weight) { 
		Point_3 r; 
//		float t;
//		assert(weight <= 1);
		r.x = x*(1-weight) + b->x*weight; 
		r.y = y*(1-weight) + b->y*weight; 
		r.z = z*(1-weight) + b->z*weight; 
		//r.nx = nx*(1-weight) + b->nx*weight; 
		//r.ny = ny*(1-weight) + b->ny*weight; 
		//r.nz = nz*(1-weight) + b->nz*weight; 
		//t = r.nx * r.nx + r.ny * r.ny + r.nz * r.nz;
		//t = sqrt(t);
		//r.nx /= t; 	r.ny /= t; 	r.nz /= t;		
		return r; 
	}
	double Distance(Point_3 p) { 
		double d,dx,dy,dz; dx = (this->x - p.x);
		dy = (this->y - p.y); dz = (this->z - p.z); d = dx*dx + dy*dy + dz*dz;
		return(sqrt(d));
	};

	double Distance(Point_3 *p) { 
		double dx,dy,dz; dx = (this->x - p->x);
		dy = (this->y - p->y); dz = (this->z - p->z); 
		return(sqrt(dx*dx + dy*dy + dz*dz));
	};
 
#include "math.h"
#include "transform.h"
#include "Point_3.h"

void Point_3 :: TransformPoint(Transform4x4 trn)
{
	double tmp[4], res[3]={3*0}; int i,j;
	tmp[0] = x; tmp[1] = y; tmp[2] = z; tmp[3] = 1;
	for(i=0; i<3; i++) for(j=0; j<4; j++) res[i] += tmp[j]*trn.matrix[i][j];
	x = res[0]; y = res[1]; z =res[2];
}

void Point_3 :: Set(double **pts, int i)
{
	x = pts[0][i]; y = pts[1][i]; z = pts[2][i];
}

void Point_3 :: Set(int i, double d)
{
	if(i == 0) x = d;
	else if(i == 1) y = d;
	else if(i == 2) z = d;
}

void Point_3 :: Set(double *v)
{
	x = v[0]; y = v[1]; z = v[2];
}

void Point_3 :: CopyTo(double **pts, int i)
{
	pts[0][i] = x; pts[1][i] = y; pts[2][i] = z;
}

Point_3 Point_3 :: Get_Normal(Point_3 v)
{
	Point_3 n, tmp;
	tmp.x = 0.0; tmp.y = 0.0; tmp.z = -1.0;
	if(fabs(tmp * v) > 0.99) {tmp.x = 0.0; tmp.y = -1.0; tmp.z = 0.0;}
	n = v >> tmp;
	n = n / ~n;
	return n;
}
*/