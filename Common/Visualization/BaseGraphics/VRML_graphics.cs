using System.Drawing;
using System.IO.Compression;
using System.Text;
using Belok.Common.Geometry;

namespace Belok.Common.Visualization.BaseGraphics
{
    public class VRML_graphics : Base_graphics
    {
        public override void open(string filename)
        {
            fs = new FileStream(filename, FileMode.Create);
            Compress = new GZipStream(fs, CompressionMode.Compress);
            writer = new StreamWriter(Compress, Encoding.UTF8);
        }
        public override void close()
        {
            writer.Flush();
            writer.Close();
            fs.Close();
            if (Compress != null)
                Compress.Close();
        }

        public override void WriteHeader()
        {
            writer.WriteLine("#VRML V2.0 utf8");
            //writer.WriteLine("Background {skyColor 0.500000 0.500000 0.500000}");
            writer.WriteLine("Background {skyColor 0.470588 0.666666 0.999999}");
            writer.WriteLine("");
            writer.WriteLine("NavigationInfo {");
            writer.WriteLine("type [\"EXAMINE\",\"FLY\",\"WALK\"]");
            writer.WriteLine("speed 4.0");
            writer.WriteLine("headlight TRUE}");
            writer.WriteLine("");
        }
        public override void DefaultViewpoint()
        {
            writer.WriteLine("Viewpoint {");
            writer.WriteLine("fieldOfView 0.523599");
            writer.WriteLine("position 0.000000 0.000000 -80.000000");
            writer.WriteLine("description \"Default View\"");
            writer.WriteLine("orientation 0 1 0 3.14159}");
            writer.WriteLine("");
        }
        public override void Viewpoint(double[] x, double Zdist)
        {
            writer.WriteLine("Viewpoint {");
            writer.WriteLine("fieldOfView 0.523599");
            writer.WriteLine("position {0} {1} {2}", x[0], x[1], x[2] - Zdist);
            writer.WriteLine("description \"Default View\"");
            writer.WriteLine("orientation 0 1 0 3.14159}");
            writer.WriteLine("");
        }
        public void AxisesEx(double dx, double dy, double dz)
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
        public override void SetColor(Color col)
        {
            color.r = col.R / 255.0;
            color.g = col.G / 255.0;
            color.b = col.B / 255.0;
        }
        public override void SetCenter(Point_3 p)
        {
            p.CopyTo(center);
        }
        public override void VRML_line(Point_3 p1, Point_3 p2)
        {
            double[] x1 = new double[3];
            //x1[0] = p1.x;
            //x1[1] = p1.y;
            //x1[2] = p1.z;
            double[] x2 = new double[3];
            //x2[0] = p2.x;
            //x2[1] = p2.y;
            //x2[2] = p2.z;
            p1.CopyTo(x1);
            p2.CopyTo(x2);
            VRML_line(x1, x2);
        }
        public override void VRML_line(double[] x1, double[] x2)
        {
            writer.WriteLine("Transform {");
            //writer.WriteLine(" translation {0} {1} {2}",x[0],x[1],x[2]);
            //writer.WriteLine(" rotation 0 0 1 0 scale 1 1 1");
            writer.WriteLine(" children [Shape {appearance Appearance {material Material {");
            writer.WriteLine(" }}geometry IndexedLineSet {");
            writer.WriteLine(" coord DEF VTKcoordinates Coordinate {point [");
            writer.WriteLine(" {0} {1} {2},", x1[0], x1[1], x1[2]);
            writer.WriteLine(" {0} {1} {2},", x2[0], x2[1], x2[2]);
            writer.WriteLine(" ]} color DEF VTKcolors Color {color [");
            writer.WriteLine(" {0} {1} {2},", color.r, color.g, color.b);
            writer.WriteLine(" {0} {1} {2},", color.r, color.g, color.b);
            writer.WriteLine(" ]}coordIndex  [");
            writer.WriteLine(" 0, 1, -1,");
            writer.WriteLine(" ]}}]}");
        }
        public override void VRML_sphere(double r, double[] xc)
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
        public override void VRML_cylinder(double rad, Point_3 p1, Point_3 p2)
        {
            double[] x1 = new double[3];
            double[] x2 = new double[3];
            p1.CopyTo(x1);
            p2.CopyTo(x2);
            VRML_cylinder(rad, x1, x2);
        }

        public override void VRML_cylinder(double rad, double[] x, double[] y)
        {
            int k; double h = 0;
            double[] xc = new double[3];
            double[] v = new double[3];
            double[] r = new double[4];
            for (k = 0; k < 3; k++)
            {
                xc[k] = 0.5 * (x[k] + y[k]);
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
        public override void VRML_cone(double rad, double[] x, double[] y)
        {
            int k; double h = 0;
            double[] xc = new double[3];
            double[] v = new double[3];
            double[] r = new double[4];
            for (k = 0; k < 3; k++) xc[k] = 0.5 * (x[k] + y[k]);
            for (k = 0; k < 3; k++) { v[k] = y[k] - x[k]; h += v[k] * v[k]; }
            h = Math.Sqrt(h);
            rotate(v, r);
            writer.WriteLine("Transform {");
            writer.WriteLine(" translation {0} {1} {2}", xc[0], xc[1], xc[2]);
            writer.WriteLine(" rotation {0} {1} {2} {3}", r[0], r[1], r[2], r[3]);
            writer.WriteLine(" children [Shape {appearance Appearance {material Material {");
            writer.WriteLine(" transparency {0}", transp);
            writer.WriteLine(" diffuseColor {0} {1} {2}", color.r, color.g, color.b);
            writer.WriteLine(" }}geometry Cone {");
            writer.WriteLine(" height {0}", h);
            writer.WriteLine(" bottomRadius {0}", rad);
            writer.WriteLine(" }}]}");
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
        public override void VRML_text(string str, double[] x)
        {
            double[] y = new double[3];
            writer.WriteLine("Transform {children [Transform {");
            writer.WriteLine(" translation {0} {1} {2}", x[0], x[1], x[2]);
            writer.WriteLine(" children [Billboard {axisOfRotation 0 0 0");
            writer.WriteLine(" children Shape {appearance Appearance {material Material {");
            writer.WriteLine(" diffuseColor {0} {1} {2}", color.r, color.g, color.b);
            writer.WriteLine(" }}geometry Text {");
            writer.WriteLine(" string \"{0}\"", str);
            writer.WriteLine(" fontStyle FontStyle{");
            writer.WriteLine(" size {0} spacing {1}", default_text_size, default_text_spacing);
            writer.WriteLine(" }}}}]}]}");
        }
        public override void VRML_surface_mesh(dTriangle[] dTriangles, int ndTriangles)
        {
            for (int k = 0; k < ndTriangles; k++)
            {
                VRML_line(dTriangles[k].p[0], dTriangles[k].p[1]);
                VRML_line(dTriangles[k].p[1], dTriangles[k].p[2]);
                VRML_line(dTriangles[k].p[2], dTriangles[k].p[0]);
            }
        }
        public override void VRML_surface_normal(dTriangle[] dTriangles, int ndTriangles)
        {
            double d = 2.0;
            for (int k = 0; k < ndTriangles; k++)
            {
                VRML_line(dTriangles[k].p[0], dTriangles[k].p[0] + dTriangles[k].n[0] / d);
                VRML_line(dTriangles[k].p[1], dTriangles[k].p[1] + dTriangles[k].n[1] / d);
                VRML_line(dTriangles[k].p[2], dTriangles[k].p[2] + dTriangles[k].n[2] / d);
            }
        }
        public override void VRML_grid(double[] center, double grid, int ngrid_x, int ngrid_y, int ngrid_z)
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
        public override void VRML_surface_solid(dTriangle[] dTriangles, int ndTriangles, Point_3[] SurfacePoints, Point_3[] SurfaceNormals, int nSurfacePoints)
        {
            double[] x = new double[3];
            writer.WriteLine("Transform{");
            writer.WriteLine(" children [Shape {appearance Appearance {material Material {");
            writer.WriteLine(" transparency {0}", transp);
            writer.WriteLine(" diffuseColor {0} {1} {2}", color.r, color.g, color.b);
            writer.WriteLine(" }}geometry IndexedFaceSet {");
            writer.WriteLine(" solid FALSE");
            //writer.WriteLine(" solid FALSE normalPerVertex FALSE");
            writer.WriteLine(" coord Coordinate {");
            writer.WriteLine(" point [");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine(" {0} {1} {2},", SurfacePoints[i].x, SurfacePoints[i].y, SurfacePoints[i].z);
            writer.WriteLine(" ]}");
            //
            writer.WriteLine("normal Normal {");
            writer.WriteLine(" vector [");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine(" {0} {1} {2},", SurfaceNormals[i].x, SurfaceNormals[i].y, SurfaceNormals[i].z);
            //for (int i = 0; i < ndTriangles; i++)
            //    writer.WriteLine(" {0} {1} {2},", dTriangles[i].norm.x, dTriangles[i].norm.y, dTriangles[i].norm.z);
            writer.WriteLine(" ]}");
            //
            writer.WriteLine("colorPerVertex TRUE color Color {");
            writer.WriteLine(" color [");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine(" {0} {1} {2},", color.r, color.g, color.b);
            writer.WriteLine(" ]}");
            writer.WriteLine(" coordIndex  [");
            for (int i = 0; i < ndTriangles; i++)
                //    writer.WriteLine(" {0}, {1}, {2}, -1,", 3 * i, 3 * i + 1, 3 * i + 2);
                writer.WriteLine(" {0}, {1}, {2}, -1,", dTriangles[i].pointindex[0], dTriangles[i].pointindex[1], dTriangles[i].pointindex[2]);
            writer.WriteLine(" ]}}]}");
        }
    }
}
