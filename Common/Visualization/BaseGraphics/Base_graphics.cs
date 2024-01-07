using System.IO.Compression;
using Belok.Common.Geometry;

namespace Belok.Common.Visualization.BaseGraphics
{
    public class VRML_color
    {
        public double r;
        public double g;
        public double b;

        public VRML_color()
        {
            r = 0;
            g = 0;
            b = 0;
        }

        public VRML_color(double R, double G, double B)
        {
            r = R;
            g = G;
            b = B;
        }
    }
    public class Base_graphics
    {
        public FileStream fs;
        public GZipStream Compress;
        public StreamWriter writer;
        public double[] center = new double[3];
        public VRML_color color = new VRML_color();
        public double transp = 0.0;
        public double default_text_size = 0.5;
        public double default_text_spacing = 0.1;

        public virtual void open(string filename)
        {
        }
        public virtual void close()
        {
        }
        public virtual void WriteHeader()
        {
        }
        public virtual void DefaultViewpoint()
        {
        }
        public virtual void Viewpoint(double[] x, double Zdist)
        {
        }
        public virtual void Axises(double dx, double dy, double dz)
        {
            double[] OO = new double[3] { 0.0, 0.0, 0.0 };
            double[] XO = new double[3] { dx, 0.0, 0.0 };
            double[] YO = new double[3] { 0.0, dy, 0.0 };
            double[] XYO = new double[3] { dx, dy, 0.0 };
            double[] OZ = new double[3] { 0.0, 0.0, dz };
            double[] XZ = new double[3] { dx, 0.0, dz };
            double[] YZ = new double[3] { 0.0, dy, dz };
            double[] XYZ = new double[3] { dx, dy, dz };
            SetColor(Color.Red);
            VRML_line(OO, XO);
            VRML_line(OZ, XZ);
            VRML_line(YO, XYO);
            VRML_line(YZ, XYZ);
            SetColor(Color.Green);
            VRML_line(OO, YO);
            VRML_line(OZ, YZ);
            VRML_line(XO, XYO);
            VRML_line(XZ, XYZ);
            SetColor(Color.Blue);
            VRML_line(OO, OZ);
            VRML_line(XO, XZ);
            VRML_line(YO, YZ);
            VRML_line(XYO, XYZ);
            SetColor(Color.White);
            default_text_size = 2.0;
            VRML_text("Origin", OO);
        }
        public virtual void SetColor(Color col)
        {
            color.r = col.R / 255.0;
            color.g = col.G / 255.0;
            color.b = col.B / 255.0;
        }
        public virtual void SetCenter(Point_3 p)
        {
            //p.CopyTo(center);
            center[0] = p.x;
            center[1] = p.y;
            center[2] = p.z;
        }
        public virtual void SetCenter(double[] xc)
        {
            center[0] = xc[0];
            center[1] = xc[1];
            center[2] = xc[2];
        }
        public virtual void VRML_line(Point_3 p1, Point_3 p2)
        {
        }
        public virtual void VRML_line(double[] x1, double[] x2)
        {
        }
        public virtual void VRML_sphere(double r, double[] xc)
        {
            double[] x = new double[3] { 0, 0, 0 };
            for (int k = 0; k < 3; k++) x[k] = xc[k];
            writer.WriteLine("Transform{");
            writer.WriteLine(" translation {0} {1} {2}", x[0], x[1], x[2]);
            writer.WriteLine(" children [Shape {appearance Appearance {material Material {");
            writer.WriteLine(" transparency {0}", transp);
            writer.WriteLine(" diffuseColor {0} {1} {2}", color.r, color.g, color.b);
            writer.WriteLine(" }}geometry Sphere {");
            writer.WriteLine(" radius {0}", r);
            writer.WriteLine(" }}]}\n");
        }
        public virtual void VRML_cylinder(double rad, Point_3 p1, Point_3 p2)
        {
            double[] x1 = new double[3];
            double[] x2 = new double[3];
            p1.CopyTo(x1);
            p2.CopyTo(x2);
            VRML_cylinder(rad, x1, x2);
        }

        public virtual void VRML_cylinder(double rad, double[] x, double[] y)
        {
            int k; double h = 0;
            double[] xc = new double[3];
            double[] v = new double[3];
            double[] r = new double[4];
            for (k = 0; k < 3; k++)
            {
                xc[k] = 0.5 * (x[k] + y[k]);
                //xc[k] = 0.5 * (x[k] + y[k]);
            }
            for (k = 0; k < 3; k++)
            {
                v[k] = y[k] - x[k]; h += v[k] * v[k];
            }
            h = Math.Sqrt(h);
            rotate(v, r);
            writer.WriteLine("Transform {");
            writer.WriteLine(" translation {0} {1} {2}", xc[0], xc[1], xc[2]);
            writer.WriteLine(" rotation {0} {1} {2} {3}", r[0], r[1], r[2], r[3]);
            writer.WriteLine(" children [Shape {appearance Appearance {material Material {");
            writer.WriteLine(" transparency {0}", transp);
            writer.WriteLine(" diffuseColor {0} {1} {2}", color.r, color.g, color.b);
            writer.WriteLine(" }}geometry Cylinder {");
            writer.WriteLine(" height {0}", h);
            writer.WriteLine(" radius {0}", rad);
            writer.WriteLine(" }}]}\n");
        }
        public virtual void VRML_cone(double rad, double[] x, double[] y)
        {
        }
        void rotate(double[] v, double[] r)
        {
            // v - any vector
            // r - VRML rotate vector. (r[0], r[1], r[2]) defines axis of rotation. r[3] - rotate angle
            // task: calculate r to superimpose Y axis (0, 1, 0) with vector v
            Point_3 p = new Point_3(v);
            double norm = ~p;
            if (norm < double.Epsilon) { p.x = 0; p.y = 1; p.z = 0; }
            else p = p / norm;
            Point_3 y = new Point_3(0.0, 1.0, 0.0);
            Point_3 axis = y ^ p;
            r[0] = axis.x; r[1] = axis.y; r[2] = axis.z;
            double cos_a = y * p;
            if (cos_a > 1.0) cos_a = 1.0;
            if (cos_a < -1.0) cos_a = -1.0;
            r[3] = Math.Acos(cos_a);
        }
        public virtual void VRML_text(string str, double[] x)
        {
        }
        public virtual void VRML_surface_mesh(dTriangle[] dTriangles, int ndTriangles)
        {
            for (int k = 0; k < ndTriangles; k++)
            {
                VRML_line(dTriangles[k].p[0], dTriangles[k].p[1]);
                VRML_line(dTriangles[k].p[1], dTriangles[k].p[2]);
                VRML_line(dTriangles[k].p[2], dTriangles[k].p[0]);
            }
        }
        public virtual void VRML_surface_normal(dTriangle[] dTriangles, int ndTriangles)
        {
            double d = 2.0;
            for (int k = 0; k < ndTriangles; k++)
            {
                Point_3 center = new Point_3();
                center.x = (dTriangles[k].p[0].x + dTriangles[k].p[1].x + dTriangles[k].p[2].x) / 3.0;
                center.y = (dTriangles[k].p[0].y + dTriangles[k].p[1].y + dTriangles[k].p[2].y) / 3.0;
                center.z = (dTriangles[k].p[0].z + dTriangles[k].p[1].z + dTriangles[k].p[2].z) / 3.0;
                //VRML_line(dTriangles[k].p[0], dTriangles[k].p[0] + dTriangles[k].n[0] / d);
                //VRML_line(dTriangles[k].p[1], dTriangles[k].p[1] + dTriangles[k].n[1] / d);
                //VRML_line(dTriangles[k].p[2], dTriangles[k].p[2] + dTriangles[k].n[2] / d);
                VRML_line(center, center + dTriangles[k].norm / d);
            }
        }
        public virtual void VRML_grid(double[] center, double grid, int ngrid_x, int ngrid_y, int ngrid_z)
        {
            double[] x1 = new double[3];
            double[] x2 = new double[3];
            for (int iz = -ngrid_z; iz <= ngrid_z; iz++)
            {
                x1[2] = iz * grid;
                x2[2] = iz * grid;
                for (int iy = -ngrid_y; iy <= ngrid_y; iy++)
                {
                    x1[1] = iy * grid;
                    x2[1] = iy * grid;
                    x1[0] = -grid * ngrid_x;
                    x2[0] = grid * ngrid_x;
                    VRML_line(x1, x2);
                }
                for (int ix = -ngrid_x; ix <= ngrid_x; ix++)
                {
                    x1[1] = -grid * ngrid_y;
                    x2[1] = grid * ngrid_y;
                    x1[0] = ix * grid;
                    x2[0] = ix * grid;
                    VRML_line(x1, x2);
                }
            }
        }
        public virtual void VRML_surface_solid(dTriangle[] dTriangles, int ndTriangles, Point_3[] SurfacePoints, Point_3[] SurfaceNormals, int nSurfacePoints)
        {
        }
    }
}
