using System;

namespace Belok.Common.PDB
{
    [Serializable]
    public class Atom
    {
        public double[] pts = new double[3];
        public double t;
        public char element;                    //protein atom C, N, O, S
        public char[] element2 = new char[2];   //ligandatom C, N, O, S, MG
        public string name;                     //CA, OG1, C21
        public int atomnumber;
        public Atom(double x, double y, double z)
        {
            pts[0] = x;
            pts[1] = y;
            pts[2] = z;
        }
        public Atom(double[] ptsIni)
        {
            for(int i = 0; i < 3; i++)
                pts[i] = ptsIni[i];
        }
        public double Dist2(Atom atm)
        {
            double dist2 = (pts[0] - atm.pts[0]) * (pts[0] - atm.pts[0]) + (pts[1] - atm.pts[1]) * (pts[1] - atm.pts[1]) + (pts[2] - atm.pts[2]) * (pts[2] - atm.pts[2]);
            return dist2;
        }
    }
}
