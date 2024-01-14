using System;
using System.Text;
using Belok.Common.Geometry;

namespace Belok.Common.PDB
{
    [Serializable]
    public class DNA
    {
        public string NAME;         //one character chain name - A
        public int AALower;         //minimum residue number - 32
        public int AAUpper;         //maximum residue number -522
        public char[] SEQRES;       //chain sequence - AGGAGCU
        public int DNA_RESIDUES;    //number of residues
        public Residue[] residues;  //residues
        public int DNA_ATOMS;       //number of atoms
        public Atom[] atoms;        //atoms
        public int nresidues = 0;          //size of temporary residue array
        public int[] ResidueList = null;   //temporary residue array

        public DNA()
        {
            DNA_RESIDUES = 0;
            DNA_ATOMS = 0;
        }
        public void GetSpace(out Point_3 pmin, out Point_3 pmax)
        {
            bool start = true;
            double[] xmin = new double[3];
            double[] xmax = new double[3];
            double[] x = new double[3];
            for (int k = 0; k < DNA_RESIDUES; k++)
            {
                Residue res = residues[k];
                if (res == null)
                    continue;
                int index = res.offset;
                int size = pdb.AA.sizeList[res.type];
                for (int j = 0; j < size; j++)
                {
                    if (!res.complete && j != 0) //P
                        continue;
                    for (int i = 0; i < 3; i++)
                    {
                        if (start)
                        {
                            xmin[i] = atoms[index + j].pts[i];
                            xmax[i] = xmin[i];
                        }
                        else
                        {
                            x[i] = atoms[index + j].pts[i];
                            if (x[i] < xmin[i])
                                xmin[i] = x[i];
                            if (x[i] > xmax[i])
                                xmax[i] = x[i];
                        }
                    }
                    start = false;
                }
            }
            pmin = new Point_3(xmin);
            pmax = new Point_3(xmax);
        }
    }
}
