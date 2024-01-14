using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belok.Common.PDB
{
    enum PdbItemType { PROTEIN = 0, LIGAND = -1, WATER = -2, METAL = -3, RNA = -4, DNA = -5, UNKNOWN = -6 };
    public enum ChainType
    {
        Protein = 0,
        Rna = 1,
        Dna = 2
    };

    [Serializable]
    public class pdb
    {
        public string PDBCODE;
        public static int DISTANCE_CHUNK = 100;
        public static Aminoacids AA = new Aminoacids();     //AA Aminiacid
        public static RNA_Nucleobases RNB = new RNA_Nucleobases();   //RNA Nucleobase
        public static DNA_Nucleobases DNB = new DNA_Nucleobases();   //DNA Nucleobase
        public int PDB_MOLECULES = 0;
        public Biomolecule molecule = null;
        public int PDB_CHAINS = 0;
        public Chain[] chains;
        public int PDB_RNAS = 0;
        public RNA[] rnas;
        public int PDB_DNAS = 0;
        public DNA[] dnas;
        public int PDB_LIGANDS = 0;
        public Ligand[] ligands;
        public int PDB_WATERS = 0;
        public HOH[] water;
        public pdb()
        {
            molecule = null;
        }
        public int GetChainId(string name)
        {
            for(int i = 0; i < PDB_CHAINS; i++)
            {
                if (name == chains[i].NAME)
                    return i;
            }
            for (int i = 0; i < PDB_RNAS; i++)
            {
                if (name == rnas[i].NAME)
                    return i;
            }
            for (int i = 0; i < PDB_DNAS; i++)
            {
                if (name == dnas[i].NAME)
                    return i;
            }
            return -1;
        }
        public int GetRNAId(string name)
        {
            for (int i = 0; i < PDB_RNAS; i++)
            {
                if (name == rnas[i].NAME)
                    return i;
            }
            return -1;
        }
        public int GetDNAId(string name)
        {
            for (int i = 0; i < PDB_DNAS; i++)
            {
                if (name == dnas[i].NAME)
                    return i;
            }
            return -1;
        }
        public void Connect(int atomnumber1, int[] atomnumber2, int nconnections)
        {
            int ligandId = GetLigandId(atomnumber1);
            if(ligandId >= 0)
                ligands[ligandId].Connect(atomnumber1, atomnumber2, nconnections);
        }
        public int GetLigandId(int atomnumber)
        {
            int id = -1;
            for (int i = PDB_LIGANDS - 1; i >= 0; i--)
            {
                if (atomnumber >= ligands[i].atoms[0].atomnumber)
                {
                    id = i;
                    break;
                }
            }
            return id;
        }
        public List<string> GetAllResidues(ChainType chainType, int chainIndex)
        {
            List<string> residues = new List<string>();
            if (chainType == ChainType.Protein)
            {
                Chain chain = chains[chainIndex];
                int start = -1;
                int end = -1;
                int size = 0;
                for (int i = 0; i <= chain.CHAIN_RESIDUES; i++)
                {
                    if (chain.residues[i] == null)
                    {
                        if (start >= 0)
                        {
                            ;
                        }
                        continue;
                    }
                    if (start == -1)
                        start = i;
                    if (start >= 0)
                        size++;
                }
                if (end == -1)
                    end = start + size - 1;
                string residueRange = String.Format("{0}-{1}", start, end);
                residues.Add(residueRange);
            }
            else if (chainType == ChainType.Rna)
            {
                RNA rna = rnas[chainIndex];
                int start = -1;
                int end = -1;
                int size = 0;
                for (int i = 0; i <= rna.RNA_RESIDUES; i++)
                {
                    if (rna.residues[i] == null)
                    {
                        if (start >= 0)
                        {
                            ;
                        }
                        continue;
                    }
                    if (start == -1)
                        start = i;
                    if (start >= 0)
                        size++;
                }
                if (end == -1)
                    end = start + size - 1;
                string residueRange = String.Format("{0}-{1}", start, end);
                residues.Add(residueRange);
            }
            return residues;
        }
        public string GetResidueRange(int chainIndex)
        {
            Chain chain = chains[chainIndex];
            int start = -1;
            int end = -1;
            int size = 0;
            for (int i = 0; i <= chain.CHAIN_RESIDUES; i++)
            {
                if (chain.residues[i] == null)
                {
                    if (start >= 0)
                    {
                        ;
                    }
                    continue;
                }
                if (start == -1)
                    start = i;
                if (start >= 0)
                    size++;
            }
            if (end == -1)
                end = start + size - 1;
            string residueRange = String.Format("{0}-{1}", start, end);
            return residueRange;
        }
        public string GetDefaultDomain(string chainName)
        {
            int chainIndex = GetChainNumber(chainName);
            string residueRange = GetResidueRange(chainIndex);
            return residueRange;
        }
        public void GetChainResidues(int LigandIndex, double distance, int ChainIndex)
        {
            int residueListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex];
            chain.nresidues = 0;
            chain.ResidueList = new int[DISTANCE_CHUNK];
            Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < chain.CHAIN_RESIDUES; i++)
            {
                if (chain.residues[i] == null)
                    continue;
                int Offset = chain.residues[i].offset;
                int natoms = 0;
                if(chain.residues[i].complete)
                    natoms = AA.sizeList[chain.residues[i].type];
                for (int j = Offset; j < Offset + natoms; j++)
                {
                    if (lig.IsNear(chain.atoms[j].pts, distance))
                    {
                        if (chain.nresidues >= residueListSize)
                        {
                            residueListSize += DISTANCE_CHUNK;
                            Array.Resize(ref chain.ResidueList, residueListSize);
                        }
                        chain.ResidueList[chain.nresidues] = i;
                        chain.nresidues++;
                        break;
                    }
                }
            }
        }
        public void GetChainResiduesByLigand(Ligand lig, double distance, int ChainIndex)
        {
            int residueListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex];
            chain.nresidues = 0;
            chain.ResidueList = new int[DISTANCE_CHUNK];
            //Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < chain.CHAIN_RESIDUES; i++)
            {
                if (chain.residues[i] == null)
                    continue;
                int Offset = chain.residues[i].offset;
                int natoms = 0;
                if (chain.residues[i].complete)
                    natoms = AA.sizeList[chain.residues[i].type];
                for (int j = Offset; j < Offset + natoms; j++)
                {
                    if (lig.IsNear(chain.atoms[j].pts, distance))
                    {
                        if (chain.nresidues >= residueListSize)
                        {
                            residueListSize += DISTANCE_CHUNK;
                            Array.Resize(ref chain.ResidueList, residueListSize);
                        }
                        chain.ResidueList[chain.nresidues] = i;
                        chain.nresidues++;
                        break;
                    }
                }
            }
        }
        public void GetChainResidues(double[] xc, double distance, int ChainIndex)
        {
            int residueListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex];
            chain.nresidues = 0;
            chain.ResidueList = new int[DISTANCE_CHUNK];
            for (int i = 0; i < chain.CHAIN_RESIDUES; i++)
            {
                if (chain.residues[i] == null)
                    continue;
                int Offset = chain.residues[i].offset;
                int natoms = 0;
                if (chain.residues[i].complete)
                    natoms = AA.sizeList[chain.residues[i].type];
                for (int j = Offset; j < Offset + natoms; j++)
                {
                    if (IsNear(chain.atoms[j].pts, xc, distance))
                    {
                        if (chain.nresidues >= residueListSize)
                        {
                            residueListSize += DISTANCE_CHUNK;
                            Array.Resize(ref chain.ResidueList, residueListSize);
                        }
                        chain.ResidueList[chain.nresidues] = i;
                        chain.nresidues++;
                        break;
                    }
                }
            }
        }
        public void GetChainContactResidues(double distance, int ChainIndex1, int ChainIndex2)
        {
            int residueListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex1];
            chain.nresidues = 0;
            chain.ResidueList = new int[DISTANCE_CHUNK];
            Chain chain2 = chains[ChainIndex2];
            double[] xc = new double[3];
            for (int i = 0; i < chain.CHAIN_RESIDUES; i++)
            {
                if (chain.residues[i] == null)
                    continue;
                Atom Ca1 = chain.atoms[chain.residues[i].offset + 1];
                for (int k = 0; k < chain2.CHAIN_RESIDUES; k++)
                {
                    if (chain2.residues[k] == null)
                        continue;
                    Atom Ca2 = chain2.atoms[chain2.residues[k].offset + 1];
                    if (IsNear(Ca1.pts, Ca2.pts, distance))
                    {
                        if (chain.nresidues >= residueListSize)
                        {
                            residueListSize += DISTANCE_CHUNK;
                            Array.Resize(ref chain.ResidueList, residueListSize);
                        }
                        chain.ResidueList[chain.nresidues] = i;
                        chain.nresidues++;
                        break;
                    }
                }
            }
        }
        public void GetRNAResidues(int LigandIndex, double distance, int RNAIndex)
        {
            int residueListSize = DISTANCE_CHUNK;
            RNA rna = rnas[RNAIndex];
            rna.nresidues = 0;
            rna.ResidueList = new int[DISTANCE_CHUNK];
            Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < rna.RNA_RESIDUES; i++)
            {
                if (rna.residues[i] == null)
                    continue;
                int Offset = rna.residues[i].offset;
                int natoms = RNB.sizeList[rna.residues[i].type];
                for (int j = Offset; j < Offset + natoms; j++)
                {
                    if (lig.IsNear(rna.atoms[j].pts, distance))
                    {
                        if (rna.nresidues >= residueListSize)
                        {
                            residueListSize += DISTANCE_CHUNK;
                            Array.Resize(ref rna.ResidueList, residueListSize);
                        }
                        rna.ResidueList[rna.nresidues] = i;
                        rna.nresidues++;
                        break;
                    }
                }
            }
        }
        public void GetDNAResidues(int LigandIndex, double distance, int DNAIndex)
        {
            int residueListSize = DISTANCE_CHUNK;
            DNA dna = dnas[DNAIndex];
            dna.nresidues = 0;
            dna.ResidueList = new int[DISTANCE_CHUNK];
            Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < dna.DNA_RESIDUES; i++)
            {
                if (dna.residues[i] == null)
                    continue;
                int Offset = dna.residues[i].offset;
                int natoms = DNB.sizeList[dna.residues[i].type];
                for (int j = Offset; j < Offset + natoms; j++)
                {
                    if (lig.IsNear(dna.atoms[j].pts, distance))
                    {
                        if (dna.nresidues >= residueListSize)
                        {
                            residueListSize += DISTANCE_CHUNK;
                            Array.Resize(ref dna.ResidueList, residueListSize);
                        }
                        dna.ResidueList[dna.nresidues] = i;
                        dna.nresidues++;
                        break;
                    }
                }
            }
        }
        public void GetChainWaters(int LigandIndex, double distance, int ChainIndex, out int[]WaterList, out int nwaters)
        {
            nwaters = 0;
            WaterList = new int[DISTANCE_CHUNK];
            int waterListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex];
            Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < PDB_WATERS; i++)
            {
                if(water[i].ChainName == chain.NAME)
                {
                    if (lig.IsNear(water[i].atom.pts, distance))
                    {
                        if (nwaters >= waterListSize)
                        {
                            waterListSize += DISTANCE_CHUNK;
                            Array.Resize(ref WaterList, waterListSize);
                        }
                        WaterList[nwaters] = i;
                        nwaters++;

                    }
                 }
            }
        }
        public void GetChainWatersByList(double waterDistance, int ChainIndex, out int[] WaterList, out int nwaters)
        {
            nwaters = 0;
            WaterList = new int[DISTANCE_CHUNK];
            int waterListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex];
            for (int i = 0; i < PDB_WATERS; i++)
            {
                if (water[i].ChainName == chain.NAME)
                {
                    bool found = false;
                    for (int k = 0; k < chain.nresidues; k++)
                    {
                        int residue = chain.ResidueList[k];
                        int Offset = chain.residues[residue].offset;
                        int natoms = 0;
                        if (chain.residues[residue].complete)
                            natoms = AA.sizeList[chain.residues[residue].type];
                        for (int j = Offset; j < Offset + natoms; j++)
                        {
                            if (IsNear(chain.atoms[j].pts, water[i].atom.pts, waterDistance))
                            {
                                if (nwaters >= waterListSize)
                                {
                                    waterListSize += DISTANCE_CHUNK;
                                    Array.Resize(ref WaterList, waterListSize);
                                }
                                WaterList[nwaters] = i;
                                nwaters++;
                                found = true;
                                break;
                            }
                            if (found)
                                break;
                        }
                        if (found)
                            break;
                    }
                }
            }
        }
        public void GetChainWaters(double[] xc, double distance, int ChainIndex, out int[] WaterList, out int nwaters)
        {
            nwaters = 0;
            WaterList = new int[DISTANCE_CHUNK];
            int waterListSize = DISTANCE_CHUNK;
            Chain chain = chains[ChainIndex];
            for (int i = 0; i < PDB_WATERS; i++)
            {
                if (water[i].ChainName == chain.NAME)
                {
                    if (IsNear(water[i].atom.pts, xc, distance))
                    {
                        if (nwaters >= waterListSize)
                        {
                            waterListSize += DISTANCE_CHUNK;
                            Array.Resize(ref WaterList, waterListSize);
                        }
                        WaterList[nwaters] = i;
                        nwaters++;

                    }
                }
            }
        }
        public bool IsNear(double[] x, double[] xc, double distance)
        {
            double dx = xc[0] - x[0];
            if (Math.Abs(dx) > distance)
                return false;
            double dy = xc[1] - x[1];
            if (Math.Abs(dy) > distance)
                return false;
            double dz = xc[2] - x[2];
            if (Math.Abs(dz) > distance)
                return false;
            double dd = dx * dx + dy * dy + dz * dz;
            if (dd < distance * distance)
                return true;
            return false;
        }
        public void GetRNAWaters(int LigandIndex, double distance, int RNAIndex, out int[] WaterList, out int nwaters)
        {
            nwaters = 0;
            WaterList = new int[DISTANCE_CHUNK];
            int waterListSize = DISTANCE_CHUNK;
            RNA rna = rnas[RNAIndex];
            Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < PDB_WATERS; i++)
            {
                if (water[i].ChainName == rna.NAME)
                {
                    if (lig.IsNear(water[i].atom.pts, distance))
                    {
                        if (nwaters >= waterListSize)
                        {
                            waterListSize += DISTANCE_CHUNK;
                            Array.Resize(ref WaterList, waterListSize);
                        }
                        WaterList[nwaters] = i;
                        nwaters++;

                    }
                }
            }
        }
        public void GetDNAWaters(int LigandIndex, double distance, int DNAIndex, out int[] WaterList, out int nwaters)
        {
            nwaters = 0;
            WaterList = new int[DISTANCE_CHUNK];
            int waterListSize = DISTANCE_CHUNK;
            DNA dna = dnas[DNAIndex];
            Ligand lig = ligands[LigandIndex];
            for (int i = 0; i < PDB_WATERS; i++)
            {
                if (water[i].ChainName == dna.NAME)
                {
                    if (lig.IsNear(water[i].atom.pts, distance))
                    {
                        if (nwaters >= waterListSize)
                        {
                            waterListSize += DISTANCE_CHUNK;
                            Array.Resize(ref WaterList, waterListSize);
                        }
                        WaterList[nwaters] = i;
                        nwaters++;

                    }
                }
            }
        }
        public double[,] GetCoordinates(int[] residues, string chainName)
        {
            double[,] x = new double[3, residues.Length];
            int i = 0;
            for (int k = 0; k < PDB_CHAINS; k++)
            {
                if (chains[k].NAME == chainName)
                {
                    foreach (int iresidue in residues)
                    {
                        Residue residue = chains[k].residues[iresidue];
                        if (residue == null)
                            continue;
                        Atom atom = chains[k].atoms[residue.offset + 1];
                        x[0, i] = atom.pts[0];
                        x[1, i] = atom.pts[1];
                        x[2, i] = atom.pts[2];
                        i++;
                    }
                    break;
                }
            }          
            return x;
        }
        public bool GetCA(string chainName, int residue, double[] x)
        {
            bool ret = false;
            for (int k = 0; k < PDB_CHAINS; k++)
            {
                if (chains[k].NAME == chainName)
                {
                    if (chains[k].residues[residue] == null)
                    {
                        x[0] = 0.0;
                        x[1] = 0.0;
                        x[2] = 0.0;
                    }
                    else
                    {
                        Atom atom = chains[k].atoms[chains[k].residues[residue].offset + 1];
                        x[0] = atom.pts[0];
                        x[1] = atom.pts[1];
                        x[2] = atom.pts[2];
                        ret = true;
                    }
                }
            }
            return ret;
        }
        public bool GetC1(string chainName, int residue, double[] x)
        {
            bool ret = false;
            for (int k = 0; k < PDB_RNAS; k++)
            {
                if (rnas[k].NAME == chainName)
                {
                    if (rnas[k].residues[residue] == null)
                    {
                        x[0] = 0.0;
                        x[1] = 0.0;
                        x[2] = 0.0;
                    }
                    else
                    {
                        Atom atom = rnas[k].atoms[rnas[k].residues[residue].offset + 11];
                        x[0] = atom.pts[0];
                        x[1] = atom.pts[1];
                        x[2] = atom.pts[2];
                        ret = true;
                    }
                }
            }
            return ret;
        }
        public string GetSeq(int chainNumber, out string resList)
        {
            resList = "";
            if (chains == null || chains[chainNumber] == null)
                return "";
            return chains[chainNumber].GetSeq(out resList);
        }
        public int GetChainNumber(string sChain)
        {
            int iChain = 0;
            for (int i = 0; i < PDB_CHAINS; i++)
            {
                if (chains[i].NAME == sChain)
                {
                    iChain = i;
                    break;
                }
            }
            return iChain;
        }
        public string GetSeq(Domain domain, out string resList)
        {
            int iChain = GetChainNumber(domain.NAME);
            return chains[iChain].GetSeq(domain, out resList);
        }
    }
}
