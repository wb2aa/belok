using System;
using Belok.Common.Geometry;

namespace Belok.Common.PDB
{
    [Serializable]
    public class Chain
    {
        public string NAME;         //one character chain name - A
        public int AALower;         //minimum residue number - 32
        public int AAUpper;         //maximum residue number -522
        public char[] SEQRES;       //chain sequence - SSQRSVARMDG
        public int CHAIN_RESIDUES;  //number of residues
        public Residue[] residues;  //residues
        public int CHAIN_ATOMS;     //number of atoms
        public Atom[] atoms;        //atoms
        public int nresidues = 0;          //size of temporary residue array
        public int[] ResidueList = null;   //temporary residue array

        public Chain()
        {
            CHAIN_RESIDUES = 0;
            CHAIN_ATOMS = 0;
        }
        public void GetSpace(out Point_3 pmin, out Point_3 pmax)
        {
            bool start = true;
            double[] xmin = new double[3];
            double[] xmax = new double[3];
            double[] x = new double[3];
            for (int k = 0; k < CHAIN_RESIDUES; k++)
            {
                Residue res = residues[k];
                if (res == null)
                    continue;
                int index = res.offset;
                int size = pdb.AA.sizeList[res.type];
                for (int j = 0; j < size; j++)
                {
                    if (!res.complete && j != 1) //CA
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
        public string GetSeq(out string resList, bool withZero = false)
        {
            string seq = "";
            resList = "";
            for (int k = 0; k < CHAIN_RESIDUES; k++)
            {
                Residue res = residues[k];
                if (res == null)
                {
                    if (withZero)
                    {
                        seq += ' ';
                        resList += "," + Convert.ToString(k);
                    }
                }
                else
                {
                    seq += residues[k].AA1;
                    resList += "," + Convert.ToString(k);
                }
            }
            return seq;
        }
        public string GetSeq(Domain domain, out string resList)
        {
            string seq = "";
            resList = "";
            for (int k = 0; k < CHAIN_RESIDUES; k++)
            {
                Residue res = residues[k];
                if (res != null)
                {
                    if (domain.InDomain(k))
                    {
                        seq += residues[k].AA1;
                        resList += "," + Convert.ToString(k);
                    }
                }
            }
            return seq;
        }
        public Point_3[] GetCaPoints()
        {
            Point_3[] ca = new Point_3[residues.Length];
            for (int i = 0; i < residues.Length; i++)
            {
                Residue res = residues[i];
                if (res == null || !res.complete)
                    ca[i] = null;
                else
                    ca[i] = new Point_3(atoms[res.offset + 1].pts);
            }
            return ca;
        }
    }
}
