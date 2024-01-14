using System;
using Belok.Common.Geometry;

//TODO compare with System.Numerics.Quaternion
namespace Belok.Common.Quaternion
{
    public class Quaternion
    {
        public Transform4x4 Execute_res_list(double[,] x1, double[,] x2, int nres)
        {
            Transform4x4 transform4x4 = new Transform4x4();
            int i, k;
            for(i = 0; i < 3; i++)
            {
		        double sum = 0.0;
		        for(k = 0; k < nres; k++) sum += x1[i,k];
		        sum /= nres;
                transform4x4.translate_to[i] = sum;
	        }
	        for(i = 0; i < 3; i++)
            {
		        double sum = 0.0;
		        for(k = 0; k < nres; k++) sum += x2[i,k];
		        sum /= nres;
                transform4x4.translate_from[i] = sum;
	        }
            for(k = 0; k < nres; k++) for(i = 0; i < 3; i++)
                    x1[i, k] -= transform4x4.translate_to[i];
	        for(k = 0; k < nres; k++) for(i = 0; i < 3; i++)
                    x2[i, k] -= transform4x4.translate_from[i];
            double[,] rotate = Execute(x2, x1, nres); //x2 first!
            Rotate rot = new Rotate();
            Point_3 p1 = new Point_3(transform4x4.translate_from[0], transform4x4.translate_from[1], transform4x4.translate_from[2]);
            Point_3 p2 = new Point_3(transform4x4.translate_to[0], transform4x4.translate_to[1], transform4x4.translate_to[2]);
	        Point_3 tna = new Point_3();
            double[,] rotate_wobble = new double[3,3];
	        rot.WobbleMatrix(tna, 0, rotate_wobble);            
            rot.TramsformTRRT(p1, rotate, rotate_wobble, p2, transform4x4);           
            return transform4x4;

        }
        public double[,] Execute(double[,] xm, double[,] xf, int imov)
        {
            int i, j, k;
            double[,] rotate = new double[3,3];
            double[] dxm = new double[3];
            double[] dxp = new double[3];
            double[,] q = new double[5, 5];
            double[,] vm = new double[5, 5];
            for(i = 0; i < 5; i++) for(k = 0; k < 5; k++) q[i,k] = 0;
	        for(k = 0; k < imov; k++) { 
		        for(i = 0; i < 3; i++)
			        {dxm[i] = (xm[i,k] - xf[i,k]); dxp[i] = (xm[i,k] + xf[i,k]);}
                //diags are sums of squared cyclic coordinate differences
		        q[1,1]=q[1,1]+dxm[0]*dxm[0]+dxm[1]*dxm[1]+dxm[2]*dxm[2];
		        q[2,2]=q[2,2]+dxp[1]*dxp[1]+dxp[2]*dxp[2]+dxm[0]*dxm[0];
		        q[3,3]=q[3,3]+dxp[0]*dxp[0]+dxp[2]*dxp[2]+dxm[1]*dxm[1];
		        q[4,4]=q[4,4]+dxp[0]*dxp[0]+dxp[1]*dxp[1]+dxm[2]*dxm[2];         
                //cross differences                                            
		        q[1,2]=q[1,2]+dxp[1]*dxm[2]-dxm[1]*dxp[2];         
		        q[1,3]=q[1,3]+dxm[0]*dxp[2]-dxp[0]*dxm[2];      
		        q[1,4]=q[1,4]+dxp[0]*dxm[1]-dxm[0]*dxp[1];          
		        q[2,3]=q[2,3]+dxm[0]*dxm[1]-dxp[0]*dxp[1];            
		        q[2,4]=q[2,4]+dxm[0]*dxm[2]-dxp[0]*dxp[2];            
		        q[3,4]=q[3,4]+dxm[1]*dxm[2]-dxp[1]*dxp[2];            
	        }
            //fill the rest by transposing it onto itself
	        for(i = 0; i < 5; i++) for(j = i + 1; j < 5; j++) q[j,i] = q[i,j];
            //orthogonalization by jacobi rotation = solution of EV -problem 
	        int n = 4, nmrot; double[] dm = new double[5];
            jacobi(q, n, dm, vm, out nmrot);  
            //sort eigenvectors after eigenvalues, descending 
            eigsrt(dm, vm, n);
            //  rotate matrix
	        rotate[0,0]=vm[1,4]*vm[1,4]+vm[2,4]*vm[2,4]-vm[3,4]*vm[3,4]-vm[4,4]*vm[4,4];      
            rotate[1,0]=2*(vm[2,4]*vm[3,4]+vm[1,4]*vm[4,4]);              
            rotate[2,0]=2*(vm[2,4]*vm[4,4]-vm[1,4]*vm[3,4]);               
            rotate[0,1]=2*(vm[2,4]*vm[3,4]-vm[1,4]*vm[4,4]);                
            rotate[1,1]=vm[1,4]*vm[1,4]+vm[3,4]*vm[3,4]-vm[2,4]*vm[2,4]-vm[4,4]*vm[4,4];         
            rotate[2,1]=2*(vm[3,4]*vm[4,4]+vm[1,4]*vm[2,4]);                
            rotate[0,2]=2*(vm[2,4]*vm[4,4]+vm[1,4]*vm[3,4]);                 
            rotate[1,2]=2*(vm[3,4]*vm[4,4]-vm[1,4]*vm[2,4]);                 
	        rotate[2,2]=vm[1,4]*vm[1,4]+vm[4,4]*vm[4,4]-vm[2,4]*vm[2,4]-vm[3,4]*vm[3,4]; 
            return rotate;
        }
        #region jacobian
        public void TestJacobi()
        {
            double[,] q = new double[5, 5] {
                {0.0,0.0,0.0,0.0,0.0},
                {0.0,1365.895874,203.947403,-621.667175,-204.787903},
                {0.0,0.000000,1197.854614,-126.254913,-47.298355},
                {0.0,0.000000,0.000000,303.344360,130.721375},
                {0.0,0.000000,0.000000,0.000000,2076.552490}
            };

            int n = 4;
            int nmrot;
            double[] dm = new double[5];
            double[,] vm = new double[5, 5];
            jacobi(q, n, dm, vm, out nmrot);
            eigsrt(dm, vm, n);
        }
        void jacobi(double[,] a, int n, double[] d, double[,] v, out int nrot)
        {
            nrot = 0;

            int j, iq, ip, i;
            double tresh, theta, tau, t, sm, s, h, g, c;
            double[] b = new double[n + 1];
            double[] z = new double[n + 1];
            for (ip = 1; ip <= n; ip++)
            {
                for (iq = 1; iq <= n; iq++) v[ip, iq] = 0.0;
                v[ip, ip] = 1.0;
            }
            for (ip = 1; ip <= n; ip++)
            {
                b[ip] = d[ip] = a[ip, ip];
                z[ip] = 0.0;
            }

            nrot = 0;
            for (i = 1; i <= 50; i++)
            {
                sm = 0.0;
                for (ip = 1; ip <= n - 1; ip++)
                {
                    for (iq = ip + 1; iq <= n; iq++)
                        sm += Math.Abs(a[ip, iq]);
                }
                if (sm == 0.0)
                {
                    return;
                }
                if (i < 4)
                    tresh = 0.2 * sm / (n * n);
                else
                    tresh = 0.0;
                for (ip = 1; ip <= n - 1; ip++)
                {
                    for (iq = ip + 1; iq <= n; iq++)
                    {
                        g = 100.0 * Math.Abs(a[ip, iq]);
                        if (i > 4 && (Math.Abs(d[ip]) + g) == Math.Abs(d[ip])
                           && (Math.Abs(d[iq]) + g) == Math.Abs(d[iq]))
                            a[ip, iq] = 0.0;
                        else if (Math.Abs(a[ip, iq]) > tresh)
                        {
                            h = d[iq] - d[ip];
                            if ((Math.Abs(h) + g) == Math.Abs(h))
                                t = (a[ip, iq]) / h;
                            else
                            {
                                theta = 0.5 * h / (a[ip, iq]);
                                t = 1.0 / (Math.Abs(theta) + Math.Sqrt(1.0 + theta * theta));
                                if (theta < 0.0) t = -t;
                            }
                            c = 1.0 / Math.Sqrt(1 + t * t);
                            s = t * c;
                            tau = s / (1.0 + c);
                            h = t * a[ip, iq];
                            z[ip] -= h;
                            z[iq] += h;
                            d[ip] -= h;
                            d[iq] += h;
                            a[ip, iq] = 0.0;
                            for (j = 1; j <= ip - 1; j++)
                                ROTATE(a, j, ip, j, iq, s, tau);
                            for (j = ip + 1; j <= iq - 1; j++)
                                ROTATE(a, ip, j, j, iq, s, tau);
                            for (j = iq + 1; j <= n; j++)
                                ROTATE(a, ip, j, iq, j, s, tau);
                            for (j = 1; j <= n; j++)
                                ROTATE(v, j, ip, j, iq, s, tau);
                            ++nrot;
                        }
                    }
                }
                for (ip = 1; ip <= n; ip++)
                {
                    b[ip] += z[ip];
                    d[ip] = b[ip];
                    z[ip] = 0.0;
                }
            }
        }
        void eigsrt(double[] d, double[,] v, int n)
        {
	        int k,j,i;
	        double p;

	        for (i=1;i<n;i++)
            {
		        p=d[k=i];
		        for (j=i+1;j<=n;j++)
			        if (d[j] >= p) p=d[k=j];
		        if (k != i) {
			        d[k]=d[i];
			        d[i]=p;
			        for (j=1;j<=n;j++) {
				        p=v[j,i];
				        v[j,i]=v[j,k];
				        v[j,k]=p;
			        }
		        }
	        }
        }
        void ROTATE(double[,] a, int i, int j, int k, int l, double s, double tau)
        {
            double g=a[i,j];
            double h=a[k,l];
            a[i,j]=g-s*(h+g*tau);
            a[k,l]=h+s*(g-h*tau);
        }
        #endregion jacobian
    }
}
