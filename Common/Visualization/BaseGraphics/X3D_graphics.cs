using System;
using System.Drawing;
using System.Text;
using Belok.Common.Geometry;

namespace Belok.Common.Visualization.BaseGraphics
{
    public class X3D_color
    {
        public double r;
        public double g;
        public double b;

        public X3D_color()
        {
            r = 0;
            g = 0;
            b = 0;
        }

        public X3D_color(double R, double G, double B)
        {
            r = R;
            g = G;
            b = B;
        }
    }
    public class X3D_Html_Stereo_graphics : X3D_Html_graphics
    {
        public override void WriteHeader()
        {
            writer.WriteLine("<!doctype html>");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("<title>Stereo Test Model</title>");
            writer.WriteLine("<meta encoding=\"utf-8\">");
            writer.WriteLine("<script src=\"./x3dom/x3dom.js\"></script>");
            writer.WriteLine("<link rel=\"stylesheet\" href=\"./x3dom/x3dom.css\">");
            writer.WriteLine("");
            writer.WriteLine("<style type='text/css'>");
            writer.WriteLine("body {margin:0; padding:0; background-color:#04344c; background-color:#ddeeff; }");
            writer.WriteLine(".stereo-view {float:left; }");
            writer.WriteLine(".stereo-view x3d {float:left; }");
            writer.WriteLine("</style>");
            writer.WriteLine("<script>");
            writer.WriteLine("// Get display size");
            writer.WriteLine("var deviceOrientation = 'portrait';");
            writer.WriteLine("notFirstTime = false;");
            writer.WriteLine("//alert ('Checking device orientation and display size');");
            writer.WriteLine("while (deviceOrientation == 'portrait') {");
            writer.WriteLine("    insideWidth = window.innerWidth;");
            writer.WriteLine("    insideHeight = window.innerHeight;");
            writer.WriteLine("    if (insideWidth > insideHeight) {deviceOrientation = 'landscape';}");
            writer.WriteLine("    if (notFirstTime) {alert (\"Please oriention device to landscape and press 'OK'\");}");
            writer.WriteLine("    notFirstTime = true;");
            writer.WriteLine("}");
            writer.WriteLine("x3dWidth = Math.floor((insideWidth-12)/2);");
            writer.WriteLine("x3dHeight = insideHeight;");
            writer.WriteLine("//x3dHeight = x3dHeight/2;");
            writer.WriteLine("x3dHeight = x3dHeight - 2;");
            writer.WriteLine("</script>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("<div class='stereo-view-1'>");
            writer.WriteLine("<script>");
            writer.WriteLine("document.writeln (\"<x3d id='all_view' showStat='false' showLog='false' x='0px' y='0px' width='\"+(2*x3dWidth)+\"px' height='\"+x3dHeight+\"px'>\");");
            writer.WriteLine("var x3dDimStr = x3dWidth + ' ' + x3dHeight + ' 4';");
            writer.WriteLine("</script>");
            writer.WriteLine("<scene>");
            writer.WriteLine("<navigationInfo type='\"EXAMINE\"'></navigationInfo>");
            writer.WriteLine("<background DEF='bgnd' skyColor=\"0.47 .67 1\"></background>");
            writer.WriteLine("<transform translation='0 0 0' rotation='0 1 0 0'>");
            writer.WriteLine("<viewpoint DEF='viewpoint' id='x3d_viewpoint' position='0 0 40' orientation='0 1 0 0' zNear='0.001' zFar='600'></viewpoint>");
            writer.WriteLine("</transform>");
            writer.WriteLine("<group id='unrendered_scene' render='false'>");
            writer.WriteLine("<group DEF='scene'>");
            writer.WriteLine("<transform DEF='Viewpoint-Left' id='x3d_viewpoint' translation='0 0 0' rotation='0 1 0 0'>");
            writer.WriteLine("<transform DEF='Viewpoint-gamma' id='x3d_viewpoint_gamma' translation='0 0 0' rotation='0 1 0 0'>");
            writer.WriteLine("<transform DEF='Viewpoint-beta' id='x3d_viewpoint_beta' translation='0 0 0' rotation='1 0 0 0'>");
            writer.WriteLine("<transform DEF='Viewpoint-alpha' id='x3d_viewpoint_alpha' translation='0 0 0' rotation='0 0 1 0'>");
            string line = string.Format("{0} {1} {2}", -center[0], -center[1], -center[2]);
            writer.WriteLine("<transform translation='{0}'>  <!--account for model center != center-of-mass-->", line);
            writer.WriteLine("<scene>");
            writer.WriteLine("<navigationInfo type='\"examine\"' id=\"navType\"></navigationInfo>");
            line = string.Format("{0} {1} {2}", center[0], center[1], center[2]);
            writer.WriteLine("<Viewpoint id=\"front\" position=\"0 0 0\" centerOfRotation=\"{0}\" orientation=\"0 1 0 3.14159\" description=\"camera\"></Viewpoint>", line);
        }
        public override void close()
        {
            writer.WriteLine("</scene>");
            writer.WriteLine("</transform>");
            writer.WriteLine("</transform>");
            writer.WriteLine("</transform>");
            writer.WriteLine("</transform>");
            writer.WriteLine("</transform>");
            writer.WriteLine("</group>");
            writer.WriteLine("</group>");
            writer.WriteLine("<group DEF='left' render='true'>");
            writer.WriteLine("<shape>");
            writer.WriteLine("<appearance>");
            writer.WriteLine("<!-- The dimensions is the size of each display side -->");
            writer.WriteLine("<script>");
            writer.WriteLine("document.writeln (\"<renderedTexture interpupillaryDistance='0.3' id='rtLeft' stereoMode='LEFT_EYE' update='ALWAYS' dimensions='\" + x3dDimStr + \"' repeatS='false' repeatT='false'>\");");
            writer.WriteLine("</script>");
            writer.WriteLine("<viewpoint USE='viewpoint' containerField='viewpoint'></viewpoint>");
            writer.WriteLine("<background USE='bgnd' containerField='background'></background>");
            writer.WriteLine("<group USE='scene' containerField=\"scene\"></group>");
            writer.WriteLine("</renderedTexture>");
            writer.WriteLine("<composedShader>");
            writer.WriteLine("<field name='tex' type='SFInt32' value='0'></field>");
            writer.WriteLine("<field name='leftEye' type='SFFloat' value='1'></field>");
            writer.WriteLine("<shaderPart type='VERTEX'>");
            writer.WriteLine("attribute vec3 position;");
            writer.WriteLine("attribute vec2 texcoord;");
            writer.WriteLine("");
            writer.WriteLine("uniform mat4 modelViewProjectionMatrix;");
            writer.WriteLine("varying vec2 fragTexCoord;");
            writer.WriteLine("");
            writer.WriteLine("void main()");
            writer.WriteLine("{");
            writer.WriteLine("     vec2 pos = sign(position.xy);");
            writer.WriteLine("     fragTexCoord = texcoord;");
            writer.WriteLine("     gl_Position = vec4((pos.x/2.0)-0.5, pos.y, 0.0, 1.0);");
            writer.WriteLine("}");
            writer.WriteLine("             </shaderPart>");
            writer.WriteLine("             <!-- The division of pos.x relates to the fraction of the window that is used -->");
            writer.WriteLine("             <shaderPart DEF=\"frag\" type='FRAGMENT'>");
            writer.WriteLine("#ifdef GL_ES");
            writer.WriteLine("          precision highp float;");
            writer.WriteLine("#endif");
            writer.WriteLine("uniform sampler2D tex;");
            writer.WriteLine("uniform float leftEye;");
            writer.WriteLine("varying vec2 fragTexCoord;");
            writer.WriteLine("void main()");
            writer.WriteLine("{");
            writer.WriteLine("     gl_FragColor = texture2D(tex, fragTexCoord);");
            writer.WriteLine("}");
            writer.WriteLine("</shaderPart>");
            writer.WriteLine("</composedShader>");
            writer.WriteLine("</appearance>");
            writer.WriteLine("<plane solid=\"false\"></plane>");
            writer.WriteLine("</shape>");
            writer.WriteLine("</group> <!--End of tag for left window -->");
            writer.WriteLine("<group DEF='right'>");
            writer.WriteLine("<shape>");
            writer.WriteLine("<appearance>");
            writer.WriteLine("<script>");
            writer.WriteLine("document.writeln (\"<renderedTexture interpupillaryDistance='0.3' id='rtRight' stereoMode='RIGHT_EYE' update='ALWAYS' dimensions='\" + x3dDimStr + \"' repeatS='false' repeatT='false'>\");");
            writer.WriteLine("</script>");
            writer.WriteLine("<viewpoint USE='viewpoint' containerField='viewpoint'></viewpoint>");
            writer.WriteLine("<background USE='bgnd' containerField='background'></background>");
            writer.WriteLine("<group USE='scene' containerField=\"scene\"></group>");
            writer.WriteLine("</renderedTexture>");
            writer.WriteLine("<composedShader>");
            writer.WriteLine("<field name='tex' type='SFInt32' value='0'></field>");
            writer.WriteLine("<field name='leftEye' type='SFFloat' value='0'></field>");
            writer.WriteLine("<shaderPart type='VERTEX'>");
            writer.WriteLine("attribute vec3 position;");
            writer.WriteLine("attribute vec2 texcoord;");
            writer.WriteLine("uniform mat4 modelViewProjectionMatrix;");
            writer.WriteLine("varying vec2 fragTexCoord;");
            writer.WriteLine("void main()");
            writer.WriteLine("{");
            writer.WriteLine("vec2 pos = sign(position.xy);");
            writer.WriteLine("fragTexCoord = texcoord;");
            writer.WriteLine("gl_Position = vec4((pos.x + 1.0)/2.0, pos.y, 0.0, 1.0);");
            writer.WriteLine("}");
            writer.WriteLine("</shaderPart>");
            writer.WriteLine("<shaderPart USE=\"frag\" type='FRAGMENT'>");
            writer.WriteLine("</shaderPart>");
            writer.WriteLine("</composedShader>");
            writer.WriteLine("</appearance>");
            writer.WriteLine("<plane solid=\"false\"></plane>");
            writer.WriteLine("</shape>");
            writer.WriteLine("</group>");
            writer.WriteLine("</scene>");
            writer.WriteLine("</x3d>");
            writer.WriteLine("</div>");
            writer.WriteLine("<script language='JavaScript'>");
            writer.WriteLine("// --> Check to see if there are DeviceOrientation events in the DOM");
            writer.WriteLine("");
            writer.WriteLine("var previousAngles = {'alpha':0, 'beta':0, 'gamma':0};");
            writer.WriteLine("var vectors = {'alpha':\"0 1 0 \", 'beta':\"1 0 0 \", 'gamma':\"0 0 1 \"};");
            writer.WriteLine("var deg2rad = Math.PI / 180;");
            writer.WriteLine("if (window.DeviceOrientationEvent) {");
            writer.WriteLine("window.addEventListener('deviceorientation', function(eventData) {");
            writer.WriteLine("gamma = eventData.gamma;");
            writer.WriteLine("beta = eventData.beta;");
            writer.WriteLine("alpha = 360-eventData.alpha;");
            writer.WriteLine("rotate (alpha, 'alpha');");
            writer.WriteLine("rotate (beta, 'beta');");
            writer.WriteLine("rotate (gamma, 'gamma');");
            writer.WriteLine("}, false);");
            writer.WriteLine("} else {");
            writer.WriteLine(" alert ('No Device Motion Sensor');");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("function rotate (angle, label) {");
            writer.WriteLine("intAngle = Math.floor(angle + 0.5);");
            writer.WriteLine("if (intAngle == previousAngles[label]) {return; }");
            writer.WriteLine("previousAngles[label] = intAngle;");
            writer.WriteLine("if (label  == 'alpha') {");
            writer.WriteLine("rotateView (\"x3d_viewpoint_\"+label, vectors[label]+\" \"+intAngle*deg2rad);");
            writer.WriteLine("//		rotateView (\"x3d_viewpointR_\"+label, vectors[label]+\" \"+intAngle*deg2rad);");
            writer.WriteLine("//		rotateView (\"x3d_viewpointL_\"+label, vectors[label]+\" \"+intAngle*deg2rad);");
            writer.WriteLine("}");
            writer.WriteLine("}");
            writer.WriteLine("function rotateView (label, vector) {");
            writer.WriteLine(" view = document.getElementById(label);");
            writer.WriteLine(" view.setAttribute('set_rotation',vector);");
            writer.WriteLine("}");
            writer.WriteLine("</script>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");

            writer.Flush();
            writer.Close();
            fs.Close();
            if (Compress != null)
                Compress.Close();
        }
    }
    public class X3D_Html_graphics : X3D_graphics
    {
        public override void WriteHeader()
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("<title>X3DOM belok.org</title>");
            //writer.WriteLine("<link rel=\"stylesheet\" href=\"http://www.x3dom.org/x3dom/example/x3dom.css\">");
            //writer.WriteLine("<script src=\"http://www.x3dom.org/x3dom/example/x3dom.js\"></script>");
            writer.WriteLine("<link rel=\"stylesheet\" href=\"./x3dom/x3dom.css\">");
            writer.WriteLine("<script src=\"./x3dom/x3dom.js\"></script>");

            writer.WriteLine("<style>");
            writer.WriteLine("html {width:100%;height:100%;margin:0;padding:0;}");
            writer.WriteLine("body {width:100%;height:100%;margin:0;padding:0;}");
            writer.WriteLine("#x3d_element {width:100%;height:100%;border:none;display:block;position:relative;background-color:rgb(120,170,255);}");
            //writer.WriteLine("#toggler {position:absolute;float:left;z-index:1;top:0px;left:0px;width:10em;height:2em;border:none;background-color:#202021;color:#ccc;}");
            //writer.WriteLine("#toggler:hover{background-color:blue;}");
            writer.WriteLine("</style>");

            //writer.WriteLine("<script>");
            //writer.WriteLine("var zoomed = false;");
            //writer.WriteLine("function toggle(button)");
            //writer.WriteLine("{");
            //writer.WriteLine("var new_size;");
            //writer.WriteLine("var x3d_element;");
            //writer.WriteLine("x3d_element = document.getElementById('x3d_element');");
            //writer.WriteLine("x3d_body = document.getElementById('x3d_body')");
            //writer.WriteLine("if (zoomed)");
            //writer.WriteLine("{new_size = \"100%\";button.innerHTML = \"Unzoom\";} ");
            //writer.WriteLine("else");
            //writer.WriteLine("{new_size = \"50%\";button.innerHTML = \"Zoom\";}");
            //writer.WriteLine("zoomed = !zoomed;");
            //writer.WriteLine("x3d_element.style.width = new_size;");
            //writer.WriteLine("x3d_element.style.height = new_size;");
            //writer.WriteLine("}");
            //writer.WriteLine("</script>");
            writer.WriteLine("</head>");

            writer.WriteLine("<body id=\"x3d_body\">");
            writer.WriteLine("<x3d id=\"x3d_element\" showlog=\"false\" showstat=\"false\">");
            //writer.WriteLine("<button id=\"toggler\" onclick=\"toggle(this); return true; \">Unzoom");
            //writer.WriteLine("</button>");
            writer.WriteLine("<scene>");
            writer.WriteLine("<navigationInfo type='\"examine\"' id=\"navType\"></navigationInfo>");
        }
    }
    public class X3D_graphics : Base_graphics
    {
        public override void open(string filename)
        {
            fs = new FileStream(filename, FileMode.Create);
            writer = new StreamWriter(fs, new UTF8Encoding(false));
        }
        public override void close()
        {
            writer.WriteLine("</scene>");
            writer.WriteLine("</x3d>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");

            writer.Flush();
            writer.Close();
            fs.Close();
            if (Compress != null)
                Compress.Close();
        }

        public override void WriteHeader()
        {
            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
            writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            writer.WriteLine("<head>");
            writer.WriteLine("<script type=\"text/javascript\" src=\"./x3dom/x3dom.js\" />");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("<x3d width='1000px' height='800px'>");
            writer.WriteLine("<scene>");

            writer.WriteLine("<navigationInfo type='\"any\"' id=\"navType\"></navigationInfo>");
            writer.WriteLine("<Background DEF='SandyShallowBottom' groundAngle='0.05 1.52 1.56 1.5707' groundColor='0.2 0.2 0 0.3 0.3 0 0.5 0.5 0.3 0.1 0.3 0.4 0 0.2 0.4' skyAngle='0.04 0.05 0.1 1.309 1.570' skyColor='0.8 0.8 0.2 0.8 0.8 0.2 0.1 0.1 0.6 0.1 0.1 0.6 0.1 0.25 0.8 0.6 0.6 0.9'/>");

        }
        public override void DefaultViewpoint()
        {
        }
        public override void Viewpoint(double[] x, double Zdist)
        {
            writer.WriteLine("<Viewpoint id=\"front\" position=\"{0} {1} {2}\" centerOfRotation=\"{3} {4} {5}\" orientation=\"0 1 0 3.14159\" description=\"camera\"></Viewpoint>", x[0], x[1], x[2] - Zdist, x[0], x[1], x[2]);
        }
        public override void Axises(double dx, double dy, double dz)
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
            writer.WriteLine("<Shape>");
            writer.WriteLine("<IndexedLineSet coordIndex='0 1'>");
            writer.WriteLine("<Coordinate point='{0} {1} {2} {3} {4} {5}'/>", x1[0], x1[1], x1[2], x2[0], x2[1], x2[2]);
            writer.WriteLine("</IndexedLineSet>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material emissiveColor=\"{0} {1} {2}\"/>", color.r, color.g, color.b);
            writer.WriteLine("</Appearance>");
            writer.WriteLine("</Shape>");
        }
        public override void VRML_sphere(double r, double[] xc)
        {
            double[] x = new double[3] { 0, 0, 0 };
            for (int k = 0; k < 3; k++) x[k] = xc[k];
            writer.WriteLine("<Transform translation='{0} {1} {2}'>", xc[0], xc[1], xc[2]);
            writer.WriteLine("<Shape>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material diffuseColor=\"{0} {1} {2}\"/>", color.r, color.g, color.b);
            writer.WriteLine("</Appearance>");
            writer.WriteLine("<Sphere radius='{0}'/>", r);
            writer.WriteLine("</Shape>");
            writer.WriteLine("</Transform>");
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
            writer.WriteLine("<Transform translation='{0} {1} {2}'>", xc[0], xc[1], xc[2]);
            writer.WriteLine("<Transform rotation=\"{0} {1} {2} {3}\">", r[0], r[1], r[2], r[3]);
            writer.WriteLine("<Shape>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material diffuseColor=\"{0} {1} {2}\"/>", color.r, color.g, color.b);
            writer.WriteLine("</Appearance>");
            writer.WriteLine("<Cylinder radius=\"{0}\" height=\"{1}\"/>", rad, h);
            writer.WriteLine("</Shape>");
            writer.WriteLine("</Transform>");
            writer.WriteLine("</Transform>");
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
            writer.WriteLine("<Transform translation='{0} {1} {2}'>", xc[0], xc[1], xc[2]);
            writer.WriteLine("<Transform rotation=\"{0} {1} {2} {3}\">", r[0], r[1], r[2], r[3]);
            writer.WriteLine("<Shape>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material diffuseColor=\"{0} {1} {2}\"/>", color.r, color.g, color.b);
            writer.WriteLine("</Appearance>");
            writer.WriteLine("<Cone bottom='false' solid='false' bottomRadius=\"{0}\" height=\"{1}\"/>", rad, h);
            writer.WriteLine("</Shape>");
            writer.WriteLine("</Transform>");
            writer.WriteLine("</Transform>");
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
            string x3dStr = str.Replace("'", "");
            writer.WriteLine("<Transform translation='{0} {1} {2}'>", x[0], x[1], x[2]);
            writer.WriteLine("<Billboard axisOfRotation=\"0 0 0\">");
            writer.WriteLine("<Shape>");
            writer.WriteLine("<Text string='\"{0}\"'>", x3dStr);
            writer.WriteLine("<FontStyle justify='\"BEGIN\" \"BEGIN\"' size='{0}'/>", default_text_size);
            writer.WriteLine("</Text>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material diffuseColor=\"{0} {1} {2}\"/>", color.r, color.g, color.b);
            writer.WriteLine("</Appearance>");
            writer.WriteLine("</Shape>");
            writer.WriteLine("</Billboard>");
            writer.WriteLine("</Transform>");
            //writer.WriteLine("Transform {children [Transform {");
            //   writer.WriteLine(" translation {0} {1} {2}", x[0], x[1], x[2]);
            //writer.WriteLine(" children [Billboard {axisOfRotation 0 0 0");
            //writer.WriteLine(" children Shape {appearance Appearance {material Material {");
            //   writer.WriteLine(" diffuseColor {0} {1} {2}", color.r, color.g, color.b);
            //writer.WriteLine(" }}geometry Text {");
            //writer.WriteLine(" string \"{0}\"",str);
            //writer.WriteLine(" fontStyle FontStyle{");
            //writer.WriteLine(" size {0} spacing {1}", default_text_size, default_text_spacing);
            //   writer.WriteLine(" }}}}]}]}");
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
        public void VRML_surface_solid1(dTriangle[] dTriangles, int ndTriangles, Point_3[] SurfacePoints, Point_3[] SurfaceNormals, int nSurfacePoints)
        {
            writer.WriteLine("<Transform>");
            writer.WriteLine("<Shape>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material diffuseColor=\"{0} {1} {2}\" transparency='{3}'/>", color.r, color.g, color.b, transp);
            writer.WriteLine("</Appearance>");

            writer.WriteLine("<IndexedTriangleSet solid='false' index='");
            for (int i = 0; i < ndTriangles; i++)
                writer.WriteLine("{0} {1} {2}", dTriangles[i].pointindex[0], dTriangles[i].pointindex[1], dTriangles[i].pointindex[2]);
            writer.WriteLine("'>");
            writer.WriteLine("' solid='false'>");
            //writer.WriteLine("' solid='false' normalPerVertex='false'>");

            writer.WriteLine("<Coordinate point='");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine("{0} {1} {2},", SurfacePoints[i].x, SurfacePoints[i].y, SurfacePoints[i].z);
            writer.WriteLine("'></Coordinate>");

            writer.WriteLine("<Normal vector='");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine("{0} {1} {2},", SurfaceNormals[i].x, SurfaceNormals[i].y, SurfaceNormals[i].z);
            // (int i = 0; i < ndTriangles; i++)
            //    writer.WriteLine("{0} {1} {2},", dTriangles[i].norm.x, dTriangles[i].norm.y, dTriangles[i].norm.z);
            writer.WriteLine("'></Normal>");

            writer.WriteLine("</IndexedTriangleSet>");
            writer.WriteLine("</Shape>");
            writer.WriteLine("</Transform>");
        }
        public override void VRML_surface_solid(dTriangle[] dTriangles, int ndTriangles, Point_3[] SurfacePoints, Point_3[] SurfaceNormals, int nSurfacePoints)
        {
            writer.WriteLine("<Transform>");
            writer.WriteLine("<Shape>");
            writer.WriteLine("<Appearance>");
            writer.WriteLine("<Material diffuseColor=\"{0} {1} {2}\" transparency='{3}'/>", color.r, color.g, color.b, transp);
            writer.WriteLine("</Appearance>");

            writer.WriteLine("<IndexedTriangleSet solid='false' index='");
            //writer.WriteLine("<IndexedTriangleSet solid='false' normalPerVertex='false' index='");
            for (int i = 0; i < ndTriangles; i++)
                writer.WriteLine("{0} {1} {2}", dTriangles[i].pointindex[0], dTriangles[i].pointindex[1], dTriangles[i].pointindex[2]);
            writer.WriteLine("'>");
            //writer.WriteLine("' solid='false'>");
            //writer.WriteLine("' solid='false' normalPerVertex='false'>");

            writer.WriteLine("<Coordinate point='");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine("{0} {1} {2},", SurfacePoints[i].x, SurfacePoints[i].y, SurfacePoints[i].z);
            writer.WriteLine("'></Coordinate>");

            writer.WriteLine("<Normal vector='");
            for (int i = 0; i < nSurfacePoints; i++)
                writer.WriteLine("{0} {1} {2},", SurfaceNormals[i].x, SurfaceNormals[i].y, SurfaceNormals[i].z);
            //for (int i = 0; i < ndTriangles; i++)
            //    writer.WriteLine("{0} {1} {2},", dTriangles[i].norm.x, dTriangles[i].norm.y, dTriangles[i].norm.z);
            writer.WriteLine("'></Normal>");

            writer.WriteLine("</IndexedTriangleSet>");
            writer.WriteLine("</Shape>");
            writer.WriteLine("</Transform>");
        }
    }
}
