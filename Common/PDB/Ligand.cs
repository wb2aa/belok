using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Belok.Common.PDB
{
    [Serializable]
    public class Ligand
    {
        double FAR = 15.0;
        public string NAME;         //three characters ligand name - NAG
        public string ChainName;    //one character chain name - A
        public int ResidueNumber;   //residue number
        public int LIGAND_ATOMS;    //number of atoms
        public Atom[] atoms;        //atoms
        public int[,] connects;     //connection table
        public int[,] bond_order;   //atm1, atm2, bondorder
        //
        public int[] n_connects;	//number of neighbors for given atom
        public int LIGAND_BONDS;    //number of molecule bonds
        public int[] max_bond_order;//maximal bond order for given atom
        public int[] sp_type;		//atom type for surface potential calculation (0, 1, ... N_TYPE); See docking_energy.cpp 
        public double[] charge;		//charge for charge potential calculation (0, 1, ... N_TYPE); See docking_energy.cpp
        public int[] DA;			//Donor/Acceptor property 0,1,2,3 = No,D,A,D+A
        //
        public double[] xmin = new double[3];
        public double[] xmax = new double[3];
        public double[] xminfar = new double[3];
        public double[] xmaxfar = new double[3];
        public double[] center = new double[3];
        public int MAX_NEIGHBOURS = 10;  //Ruthenium 3p44.pdb ligand 067 valencе 10

        public Ligand()
        {
            LIGAND_ATOMS = 0;
            LIGAND_BONDS = 0;
            connects = null;
            n_connects = null;
        }
        //CONECT 1919 1920 1921 1922 1923
        public void Connect(int atomnumber1, int[] atomnumber2, int nconnections)
        {
            if (connects == null)
            {
                connects = new int[LIGAND_ATOMS, MAX_NEIGHBOURS];
                n_connects = new int[LIGAND_ATOMS];
            }
            for (int i = 0; i < LIGAND_ATOMS; i++)
            {
                if(atoms[i].atomnumber == atomnumber1)
                {
                    n_connects[i] = nconnections;
                    for (int j = 0; j < nconnections; j++)
                    {
                        bool FIND = false;
                        for (int k = 0; k < LIGAND_ATOMS; k++)
                        {
                            if (atoms[k].atomnumber == atomnumber2[j])
                            {
                                connects[i, j] = k;
                                if (i < k)
                                    LIGAND_BONDS++;
                                FIND = true;
                                break;
                            }
                        }
                        if (!FIND)
                        {
                            connects[i, j] = -1;
                            //n_connects[i]--; hydrogen
                        }
                    }
                }
            }
        }
        public void GetCenter(double[] x)
        {
           bool start = true;
           for (int k = 0; k < LIGAND_ATOMS; k++)
           {
                for (int i = 0; i < 3; i++)
                {
                    if (start)
                    {
                        xmin[i] = atoms[k].pts[i];
                        xmax[i] = xmin[i];
                    }
                    else
                    {
                        x[i] = atoms[k].pts[i];
                        if (x[i] < xmin[i])
                            xmin[i] = x[i];
                        if (x[i] > xmax[i])
                            xmax[i] = x[i];
                    }
                }
                start = false;
           }
            for (int k = 0; k < 3; k++)
            {
                xminfar[k] = xmin[k] - FAR;
                xmaxfar[k] = xmax[k] + FAR;
                center[k] = xmin[k] + 0.5 * (xmax[k] - xmin[k]);
                x[k] = center[k];
            }
        }
        public bool IsNear(double[] x, double distance)
        {
            for (int i = 0; i < LIGAND_ATOMS; i++)
            {
                double dx = atoms[i].pts[0] - x[0];
                if (Math.Abs(dx) > distance)
                    continue;
                double dy = atoms[i].pts[1] - x[1];
                if (Math.Abs(dy) > distance)
                    continue;
                double dz = atoms[i].pts[2] - x[2];
                if (Math.Abs(dz) > distance)
                    continue;
                double dd = dx * dx + dy * dy + dz * dz;
                if (dd < distance * distance)
                    return true;
            }
            return false;
        }
        public int GetAtomId(string atomname)
        {
            for (int i = 0; i < LIGAND_ATOMS; i++)
            {
                if (atoms[i].name == atomname)
                    return i;
            }
            return -1;
        }
        static int MOL_XYZ = 10;
        static int MOL_ATM = 31;
        static int MOL_CNN = 3;
        public void SetMempry()
        {
            if (LIGAND_ATOMS > 0)
            {
                atoms = new Atom[LIGAND_ATOMS];
                max_bond_order = new int[LIGAND_ATOMS];
            }
            if (LIGAND_BONDS > 0)
            {
                bond_order = new int[LIGAND_BONDS, 3];
            }
        }
        public void ReadMOL(string path)
        {
            StreamReader streamReader = new System.IO.StreamReader(path);
            ConvertMol(streamReader);
            streamReader.Close();
        }
        public void ConvertFromMolText(string molText)
        {
            Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(molText));
            StreamReader streamReader = new StreamReader(stream);
            ConvertMol(streamReader);
            streamReader.Close();
        }
        public void ConvertMol(StreamReader streamReader)
        {
            string line = "";
            for (int i = 0; i < 4; i++)
                line = streamReader.ReadLine();
            // 10  9  0  0  0  0  0  0  0  0999 V2000
            LIGAND_ATOMS = Convert.ToInt32(line.Substring(0, 3));
            LIGAND_BONDS = Convert.ToInt32(line.Substring(3, 3));
            SetMempry();
            //    2.5770   12.4710   74.9210 N   0  0  0  0  0  0  0  0  0110
            for (int i = 0; i < LIGAND_ATOMS; i++)
            {
                line = streamReader.ReadLine();
                double x = Convert.ToDouble(line.Substring(0, MOL_XYZ));
                double y = Convert.ToDouble(line.Substring(MOL_XYZ, MOL_XYZ));
                double z = Convert.ToDouble(line.Substring(2 * MOL_XYZ, MOL_XYZ));
                atoms[i] = new Atom(x, y, z);
                atoms[i].element = line[MOL_ATM];
                atoms[i].element2[0] = line[MOL_ATM];
                atoms[i].element2[1] = line[MOL_ATM + 1];
            }
            connects = new int[LIGAND_ATOMS, MAX_NEIGHBOURS];
            n_connects = new int[LIGAND_ATOMS];
            //  1  2  1  0  0  0
            for (int i = 0; i < LIGAND_BONDS; i++)
            {
                line = streamReader.ReadLine();
                int atm1Id = Convert.ToInt32(line.Substring(0, MOL_CNN));
                int atm2Id = Convert.ToInt32(line.Substring(MOL_CNN, MOL_CNN));
                int bondOrder = Convert.ToInt32(line.Substring(2 * MOL_CNN, MOL_CNN));
                //zero based
                atm1Id--;
                atm2Id--;
                bond_order[i, 0] = atm1Id;
                bond_order[i, 1] = atm2Id;
                bond_order[i, 2] = bondOrder;
                if (max_bond_order[atm1Id] < bondOrder)
                    max_bond_order[atm1Id] = bondOrder;
                if (max_bond_order[atm2Id] < bondOrder)
                    max_bond_order[atm2Id] = bondOrder;
                connects[atm1Id, n_connects[atm1Id]] = atm2Id;
                n_connects[atm1Id]++;
                connects[atm2Id, n_connects[atm2Id]] = atm1Id;
                n_connects[atm2Id]++;
            }
        }
    }
}
