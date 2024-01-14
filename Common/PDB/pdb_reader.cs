using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Belok.Common.PDB
{
    //http://structure.usc.edu/pdb/
    public class pdb_reader
    {
        int ATOM_CHUNK = 500;
        int LIGAND_CHUNK = 50;
        int LIGAND_ATOM_CHUNK = 100;
        int PDB_ITEMS_CHUNK = 50;
        int PROTEIN_RESIDUE_CHUNK = 500;
        int PDB_WATERS_CHUNK = 100;
        int[] CHAINS_ATOM_SIZE = null;
        int[] CHAINS_RESIDUE_SIZE = null;
        int[] RNA_ATOM_SIZE = null;
        int[] RNA_RESIDUE_SIZE = null;
        int[] DNA_ATOM_SIZE = null;
        int[] DNA_RESIDUE_SIZE = null;


        public pdb ReadEnt(StreamReader reader)
        {
            pdb crystal = new pdb();
            string line;
            char[] space = {' '};
            char[] space_comma_semicolon = { ' ', ',', ';' };
            char[] comma_colon = { ',', ':' };
            //string ProcessedChainName = "";
            //int ChainId;
            //int ProcessedChainId = -1;
            string[] split;
            int ChainIndex;
            int RNAIndex;
            int DNAIndex;
            int ResidueIndex;
            int CurrentResidueIndex = 0;
            int LigandIndex;
            int CurrentLigandIndex = 0;
            int CurrentChainIndex = 0;
            int ResidueAtoms = 0;
            //
            Array.Resize(ref crystal.chains, 0);
            crystal.PDB_CHAINS = 0;
            crystal.chains = new Chain[PDB_ITEMS_CHUNK];
            CHAINS_ATOM_SIZE = new int[PDB_ITEMS_CHUNK];
            CHAINS_RESIDUE_SIZE = new int[PDB_ITEMS_CHUNK];
            //
            Array.Resize(ref crystal.rnas, 0);
            crystal.PDB_RNAS = 0;
            crystal.rnas = new RNA[PDB_ITEMS_CHUNK];
            RNA_ATOM_SIZE = new int[PDB_ITEMS_CHUNK];
            RNA_RESIDUE_SIZE = new int[PDB_ITEMS_CHUNK];
            //
            Array.Resize(ref crystal.dnas, 0);
            crystal.PDB_DNAS = 0;
            crystal.dnas = new DNA[PDB_ITEMS_CHUNK];
            DNA_ATOM_SIZE = new int[PDB_ITEMS_CHUNK];
            DNA_RESIDUE_SIZE = new int[PDB_ITEMS_CHUNK];
            //
            Array.Resize(ref crystal.ligands, 0);
            crystal.PDB_LIGANDS = 0;
            crystal.ligands = new Ligand[LIGAND_CHUNK];
            //
            Array.Resize(ref crystal.water, 0);
            crystal.PDB_WATERS = 0;
            crystal.water = new HOH[PDB_WATERS_CHUNK];
            //
            crystal.molecule = null;
            bool SKIP_ATOM = false;
            int connectAtomNumber = 0;
            int[] atomnumber2 = new int[0];
            int nconnections = 0;
            for (;;)
            {
                line = reader.ReadLine();
                if (line == null)
                    break;
                #region other sections
                //COMPND   3 CHAIN: A, B;
                //if (line.StartsWith("COMPND   3 CHAIN:"))
                //{
                //    split = line.Split(space_comma_semicolon, StringSplitOptions.RemoveEmptyEntries);
                //    crystal.PDB_CHAINS = split.Length - 3;
                //    ATOM_SIZE = new int[crystal.PDB_CHAINS];
                //    RESIDUE_SIZE = new int[crystal.PDB_CHAINS];
                //    crystal.chains = new Chain[crystal.PDB_CHAINS];
                //    for (int i = 0; i < crystal.PDB_CHAINS; i++)
                //    {
                //        crystal.chains[i] = new Chain();
                //        crystal.chains[i].residues = new Residue[PROTEIN_RESIDUE_CHUNK];
                //        crystal.chains[i].SEQRES = new char[PROTEIN_RESIDUE_CHUNK];
                //        RESIDUE_SIZE[i] = PROTEIN_RESIDUE_CHUNK;
                //        crystal.chains[i].NAME = split[3 + i];
                //        ATOM_SIZE[i] = ATOM_CHUNK;
                //        crystal.chains[i].atoms = new Atom[ATOM_SIZE[i]];
                //        crystal.chains[i].CHAIN_ATOMS = 0;
                //    }
                    
                //}
                //REMARK   3   PROTEIN ATOMS            : 7129
                //if (line.StartsWith("REMARK   3   PROTEIN ATOMS"))
                //{
                //    split = line.Split(':');
                //    crystal.PROTEIN_ATOMS = Convert.ToInt32(split[1]);
                //    crystal.atoms = new Atom[crystal.PROTEIN_ATOMS + 1];
                //}
                //DBREF  1EWK A   33   522  UNP    P23385   MGR1_RAT        33    522
                //if (line.StartsWith("DBREF"))
                //{
                //    split = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                //    ChainId = crystal.GetChainId(split[2]);
                //    crystal.chains[ChainId].AALower = Convert.ToInt32(split[3]);
                //    crystal.chains[ChainId].AAUpper = Convert.ToInt32(split[4]);
                //    crystal.chains[ChainId].CHAIN_RESIDUES = crystal.chains[ChainId].AAUpper;
                //    crystal.chains[ChainId].SEQRES = new char[crystal.chains[ChainId].AAUpper + 1];
                //    char NoneAA = crystal.AA.GetNoneAA();
                //    for (int i = 0; i < crystal.chains[ChainId].AALower; i++)
                //        crystal.chains[ChainId].SEQRES[i] = NoneAA;
                //}
                //SEQRES   1 A  484  LEU SER ALA ALA SER TRP ARG THR GLN SER ILE TYR PHE               
                //if (line.StartsWith("SEQRES"))
                //{
                //    split = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                //    int Line13 = Convert.ToInt32(split[1]);
                //    int seqlenght = Convert.ToInt32(split[3]);
                //    if (ProcessedChainName != split[2])
                //    {
                //        ProcessedChainId = crystal.GetChainId(split[2]);
                //        if (crystal.chains[ProcessedChainId].CHAIN_RESIDUES < seqlenght + 1)
                //        {
                //            Array.Resize(ref crystal.chains[ProcessedChainId].SEQRES, seqlenght + 1);    
                //        }
                //        ProcessedChainName = split[2];
                //    }
                //    int n1 = crystal.chains[ProcessedChainId].AALower + (Line13 - 1) * 13;
                //    char AA1;
                //    for(int i = 0; i < split.Length - 4; i++)
                //    {
                //        AA1 = crystal.AA.GetAA1(split[i + 4]);
                //        crystal.chains[ProcessedChainId].SEQRES[n1 + i] = AA1;
                //    }
                //}
                //any required field after SEQRES and before ATOM
                //ORIGX1      1.000000  0.000000  0.000000        0.00000
                //if (line.StartsWith("ORIGX1"))
                //{
                //    ATOM_SIZE = new int[crystal.PDB_CHAINS];
                //    for (int i = 0; i < crystal.PDB_CHAINS; i++)
                //    {
                //        crystal.chains[i].residues = new Residue[crystal.chains[i].AAUpper + 1];
                //        ATOM_SIZE[i] = ATOM_CHUNK;
                //        crystal.chains[i].atoms = new Atom[ATOM_SIZE[i]];
                //        crystal.chains[i].CHAIN_ATOMS = 0;
                //    }
                //}
                #endregion other sections
                //REMARK 350 APPLY THE FOLLOWING TO CHAINS: A, B, C, D, E                         
                //REMARK 350   BIOMT1   1  1.000000  0.000000  0.000000        0.00000            
                //REMARK 350   BIOMT2   1  0.000000  1.000000  0.000000        0.00000            
                //REMARK 350   BIOMT3   1  0.000000  0.000000  1.000000        0.00000            
                //REMARK 350   BIOMT1   2 -1.000000  0.000000  0.000000        0.00000            
                //REMARK 350   BIOMT2   2  0.000000  1.000000  0.000000        0.00000            
                //REMARK 350   BIOMT3   2  0.000000  0.000000 -1.000000      102.43350
                if (line.StartsWith("REMARK 350 APPLY THE FOLLOWING TO CHAINS:"))
                {
                    split = line.Split(comma_colon, StringSplitOptions.RemoveEmptyEntries);
                    if (crystal.molecule == null)
                        crystal.molecule = new Biomolecule();
                    crystal.molecule.SetChainList(split);
                    continue;
                }
                if (line.StartsWith("REMARK 350   BIOMT"))
                {
                    string[] BIOMT = new string[3];
                    BIOMT[0] = line;
                    BIOMT[1] = reader.ReadLine();
                    BIOMT[2] = reader.ReadLine();
                    crystal.molecule.AddMatrix(BIOMT);
                    continue;
                }
                //MODEL        1
                if (line.StartsWith("MODEL"))
                {
                    split = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                    int MODEL = Convert.ToInt32(split[1]);
                    if (MODEL > 1)
                        SKIP_ATOM = true;
                }
                //ENDMDL
                if (line.StartsWith("ENDMDL"))
                {
                    SKIP_ATOM = false;
                }
                //ATOM      1  N   ARG A  36      15.624 -10.527   1.751  1.00 38.59           N
                int INSERTION = 26; //Code for insertion of residues
                if (line.StartsWith("ATOM"))
                {
                    if (SKIP_ATOM)
                        continue;
                    if (line[INSERTION] != ' ')
                        continue;
                    //split = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                    split = SplitAtom(line);
                    if (split == null)
                        continue;
                    if (split.Length != 12)
                        continue;
                    ResidueIndex = Convert.ToInt32(split[5]);
                    if (ResidueIndex <= 0)
                        continue;
                    if (split[2].StartsWith("H")) //hydrogen atom
                        continue;
                    //
                    double x = Convert.ToDouble(split[6]);
                    double y = Convert.ToDouble(split[7]);
                    double z = Convert.ToDouble(split[8]);
                    Atom atom = new Atom(x, y, z);
                    atom.t = Convert.ToDouble(split[10]);
                    if (split[11].Length > 0)
                    {
                        atom.element = split[11][0];
                        atom.element2[0] = split[11][0];
                        if (split[11].Length > 1)
                            atom.element2[1] = split[11][1];
                        else
                            atom.element2[1] = ' ';
                    }
                    else
                    {
                        atom.element = split[2][0];
                        atom.element2[0] = split[2][0];
                        atom.element2[1] = ' ';
                    }
                    atom.atomnumber = Convert.ToInt32(split[1]);
                    //
                    ChainIndex = -1;
                    RNAIndex = -1;
                    DNAIndex = -1;
                    PdbItemType ItemType = GetItemType(split[3]);
                    switch (ItemType)
                    {
                        case PdbItemType.PROTEIN:
                            ChainIndex = crystal.GetChainId(split[4]);
                            if (ChainIndex < 0)
                            {
                                ChainIndex = AddChain(crystal);
                                crystal.chains[ChainIndex].NAME = split[4];
                                crystal.chains[ChainIndex].NAME = split[4];
                                ResidueAtoms = 0;
                            }
                            if (ResidueIndex != CurrentResidueIndex)
                            {
                                if(ResidueAtoms > 0)
                                {
                                    Chain chain = crystal.chains[ChainIndex];
                                    Residue residue = chain.residues[CurrentResidueIndex];
                                    int ResidueType = residue.type;
                                    int ResidueSize = pdb.AA.sizeList[ResidueType];
                                    if (ResidueSize == ResidueAtoms)
                                        residue.complete = true;
                                    ResidueAtoms = 0;
                                }
                                AddChainResidue(crystal, CurrentResidueIndex, ResidueIndex, ChainIndex, split[3]);
                                CurrentResidueIndex = ResidueIndex;
                            }
                            AddChainAtom(crystal, ChainIndex, atom);
                            ResidueAtoms++;
                            break;
                        case PdbItemType.DNA:
                            DNAIndex = crystal.GetDNAId(split[4]);
                            if (DNAIndex < 0)
                            {
                                DNAIndex = AddDNA(crystal);
                                crystal.dnas[DNAIndex].NAME = split[4];
                                crystal.dnas[DNAIndex].NAME = split[4];
                                ResidueAtoms = 0;
                            }
                            if (ResidueIndex != CurrentResidueIndex)
                            {
                                if (ResidueAtoms > 0)
                                {
                                    DNA dna = crystal.dnas[DNAIndex];
                                    Residue residue = dna.residues[CurrentResidueIndex];
                                    int ResidueType = residue.type;
                                    int ResidueSize = pdb.DNB.sizeList[ResidueType];
                                    if (ResidueSize == ResidueAtoms)
                                        residue.complete = true;
                                    ResidueAtoms = 0;
                                }
                                AddDNAResidue(crystal, CurrentResidueIndex, ResidueIndex, DNAIndex, split[3]);
                                CurrentResidueIndex = ResidueIndex;
                            }
                            AddDNAAtom(crystal, DNAIndex, atom);
                            ResidueAtoms++;
                            break;
                        case PdbItemType.RNA:
                            RNAIndex = crystal.GetRNAId(split[4]);
                            if (RNAIndex < 0)
                            {
                                RNAIndex = AddRNA(crystal);
                                crystal.rnas[RNAIndex].NAME = split[4];
                                crystal.rnas[RNAIndex].NAME = split[4];
                                ResidueAtoms = 0;
                            }
                            if (ResidueIndex != CurrentResidueIndex)
                            {
                                if (ResidueAtoms > 0)
                                {
                                    RNA rna = crystal.rnas[RNAIndex];
                                    Residue residue = rna.residues[CurrentResidueIndex];
                                    int ResidueType = residue.type;
                                    int ResidueSize = pdb.RNB.sizeList[ResidueType];
                                    if (ResidueSize == ResidueAtoms)
                                        residue.complete = true;
                                    ResidueAtoms = 0;
                                }
                                AddRNAResidue(crystal, CurrentResidueIndex, ResidueIndex, RNAIndex, split[3]);
                                CurrentResidueIndex = ResidueIndex;
                            }
                            AddRNAAtom(crystal, RNAIndex, atom);
                            ResidueAtoms++;
                            break;
                        default:
                            continue;
                    }
                    continue;
                }
                //HETATM 7144  C1  NAG A 801      24.189  15.189 -21.399  1.00 64.70           C 
                if (line.StartsWith("HETATM"))
                {
                    if (SKIP_ATOM)
                        continue;
                    //split = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
                    split = SplitLigandAtom(line);
                    if (split == null)
                        continue;
                    if (split.Length != 12)
                    {
                        split = SplitSpecial(line);
                        if (split.Length != 12)
                            continue;
                    }
                    //if (split[2].StartsWith("H")) //hydrogen atom
                    //    continue;
                    ChainIndex = crystal.GetChainId(split[4]);
                    LigandIndex = Convert.ToInt32(split[5]);
                    if (LigandIndex <= 0)
                        continue; //6LDH
                    double x = Convert.ToDouble(split[6]);
                    double y = Convert.ToDouble(split[7]);
                    double z = Convert.ToDouble(split[8]);
                    Atom atom = new Atom(x, y, z);
                    atom.t = Convert.ToDouble(split[10]);
                    atom.name = split[2];
                    atom.element = split[11][0];
                    atom.element2[0] = split[11][0];
                    if (split[11].Length > 1)
                        atom.element2[1] = split[11][1];
                    else
                        atom.element2[1] = ' ';
                    atom.atomnumber = Convert.ToInt32(split[1]);
                    if (split[3] == "HOH")
                    {
                        crystal.water[crystal.PDB_WATERS] = new HOH();
                        crystal.water[crystal.PDB_WATERS].ChainName = split[4];
                        crystal.water[crystal.PDB_WATERS].ResidueNumber = LigandIndex;
                        crystal.water[crystal.PDB_WATERS].atom = atom;
                        crystal.PDB_WATERS++;
                        if (crystal.PDB_WATERS >= crystal.water.Count())
                        {
                            int newsize = crystal.water.Count() + PDB_WATERS_CHUNK;
                            Array.Resize(ref crystal.water, newsize);
                        }
                        continue;
                    }
                    if ((LigandIndex != CurrentLigandIndex) || (ChainIndex != CurrentChainIndex))
                    {
                        crystal.ligands[crystal.PDB_LIGANDS] = new Ligand();
                        crystal.ligands[crystal.PDB_LIGANDS].NAME = split[3];
                        crystal.ligands[crystal.PDB_LIGANDS].ChainName = split[4];
                        crystal.ligands[crystal.PDB_LIGANDS].ResidueNumber = LigandIndex;
                        crystal.ligands[crystal.PDB_LIGANDS].atoms = new Atom[LIGAND_ATOM_CHUNK];
                        crystal.PDB_LIGANDS++;
                        if (crystal.PDB_LIGANDS >= crystal.ligands.Count())
                        {
                            int newsize = crystal.ligands.Count() + LIGAND_CHUNK;
                            Array.Resize(ref crystal.ligands, newsize);
                        }
                        CurrentLigandIndex = LigandIndex;
                        CurrentChainIndex = ChainIndex;
                    }
                    crystal.ligands[crystal.PDB_LIGANDS - 1].atoms.SetValue(atom, crystal.ligands[crystal.PDB_LIGANDS - 1].LIGAND_ATOMS);
                    crystal.ligands[crystal.PDB_LIGANDS - 1].LIGAND_ATOMS++;
                    continue;
                }
                //ligand 34B in pdb 2C2S
                //CONECT 3134 3135 3137 3140 3141
                //CONECT 3134 3146
                //CONECT 3135 3134 3137 3138 3140
                //CONECT 3135 3149
                if (line.StartsWith("CONECT"))
                {
                    split = SplitConnect(line);
                    int atomnumber1 = Convert.ToInt32(split[1]);
                    if (atomnumber1 == connectAtomNumber)
                    {
                        int nconnectionsAppended = nconnections + split.Length - 2;
                        int[] atomnumber2Appended = new int[nconnectionsAppended];
                        for (int i = 0; i < nconnections; i++)
                            atomnumber2Appended[i] = atomnumber2[i];
                        for (int i = nconnections; i < nconnectionsAppended; i++)
                            atomnumber2Appended[i] = Convert.ToInt32(split[i - nconnections + 2]);
                        crystal.Connect(atomnumber1, atomnumber2Appended, nconnectionsAppended);
                    }
                    else
                    {
                        connectAtomNumber = atomnumber1;
                        nconnections = split.Length - 2;
                        atomnumber2 = new int[nconnections];
                        for (int i = 2; i < split.Length; i++)
                            atomnumber2[i - 2] = Convert.ToInt32(split[i]);
                        crystal.Connect(atomnumber1, atomnumber2, nconnections);
                    }
                    continue;
                }

            }
            for (int i = 0; i < crystal.PDB_CHAINS; i++)
            {
                Array.Resize(ref crystal.chains[i].atoms, crystal.chains[i].CHAIN_ATOMS);
            }
            Array.Resize(ref crystal.ligands, crystal.PDB_LIGANDS);
            for (int i = 0; i < crystal.PDB_LIGANDS; i++)
            {
                Array.Resize(ref crystal.ligands[i].atoms, crystal.ligands[i].LIGAND_ATOMS);
            }
            return crystal;
        }
        //0         1         2         3         4         5         6         7
        //012345678901234567890123456789012345678901234567890123456789012345678901234567
        //ATOM      1  N   ARG A  36      15.624 -10.527   1.751  1.00 38.59           N
        //pdb1t46.ent
        //ATOM    873  OD1AASP A 677      35.482  27.581  42.069  0.65 18.68           O
        //ATOM    874  OD1BASP A 677      38.493  27.560  42.782  0.35 16.55           O
        //pdb2i13.ent
        //ATOM      1  O5'  DC C   1       2.812  65.552  53.615  1.00 62.34           O   
        //t11a:
        //ATOM      1  N   GLY    36      21.834   3.628 -11.929  1.00 50.00
        private string[] SplitAtom(string line)
        {
            string ALI = line.Substring(16, 1); //Alternate location indicator
            if ((ALI != " ") && (ALI != "A"))
                return null;
            string[] split = new string[12];
            split[0] = line.Substring(0, 4);
            split[1] = line.Substring(4, 7);
            split[2] = line.Substring(11, 5);
            split[3] = line.Substring(17, 3);
            split[4] = line.Substring(21, 1);
            split[5] = line.Substring(22, 4);
            split[6] = line.Substring(30, 8);
            split[7] = line.Substring(38, 8);
            split[8] = line.Substring(46, 8);
            split[9] = line.Substring(54, 6);
            split[10] = line.Substring(60, 6);
            if (line.Length >= 78)
                split[11] = line.Substring(77, 1);
            else
                split[11] = line.Substring(13, 1);
            for(int i = 0; i < 12; i++)
            {
                split[i] = split[i].Trim();
            }
            return split;
        }
        //0         1         2         3         4         5         6         7
        //012345678901234567890123456789012345678901234567890123456789012345678901234567
        //HETATM 1069  C10AASD A 126      26.948  -1.485 -14.451  0.50 26.77           C
        //HETATM 2929 CL   YJ9 A1221      11.115  31.122  18.762  1.00 22.01          CL
        private string[] SplitLigandAtom(string line)
        {
            string ALI = line.Substring(16, 1); //Alternate location indicator
            if ((ALI != " ") && (ALI != "A"))
                return null;
            string[] split = new string[12];
            split[0] = line.Substring(0, 5);
            split[1] = line.Substring(6, 5);
            split[2] = line.Substring(11, 5);
            split[3] = line.Substring(17, 3);
            split[4] = line.Substring(21, 1);
            split[5] = line.Substring(22, 4);
            split[6] = line.Substring(26, 12);
            split[7] = line.Substring(38, 8);
            split[8] = line.Substring(46, 8);
            split[9] = line.Substring(54, 6);
            split[10] = line.Substring(60, 6);
            split[11] = line.Substring(76, 2);
            for (int i = 0; i < 12; i++)
            {
                split[i] = split[i].Trim();
            }
            return split;
        }
        //CONECT 7144  497 7145 7155
        //CONECT126091261012611
        private string[] SplitConnect(string line)
        {
            char[] space = { ' ' };
            //string[] split = line.Split(space, StringSplitOptions.RemoveEmptyEntries);
            //return split;
            string[] split = { "CONECT" };
            int StartIndex = 1;
            int Length = 5;
            int NewSize = 1;
            string newline = line.TrimEnd();
            for (; ; )
            {
                StartIndex += 5;
                if (StartIndex >= newline.Length)
                    break;
                string number = newline.Substring(StartIndex, Length);
                if (number.Length == 0)
                    break;
                Array.Resize(ref split, NewSize + 1);
                split[NewSize] = number;
                NewSize++;
            }
            return split;
        }
        //0         1         2
        //0123456789012345678901234567890
        //HETATM 2297  O   HOH A3001      28.735  38.497  -3.826  1.00 24.27           O  
        private string[] SplitSpecial(string line)
        {
            char[] space = { ' ' };
            if (line[22] != ' ')
            {
                string newline = line.Substring(0, 22) + " " + line.Substring(22);
                string[] split = newline.Split(space, StringSplitOptions.RemoveEmptyEntries);
                return split;
            }
            return line.Split(space, StringSplitOptions.RemoveEmptyEntries);
        }
        private PdbItemType GetItemType(string ResidueName)
        {
            if (ResidueName.Length == 3)
                if(Aminoacids.CheckName(ResidueName))
                    return PdbItemType.PROTEIN;
            if (ResidueName.Length == 2)
                if (DNA_Nucleobases.CheckName(ResidueName))
                    return PdbItemType.DNA;
            if (ResidueName.Length == 1)
                if (RNA_Nucleobases.CheckName(ResidueName))
                    return PdbItemType.RNA;
            return PdbItemType.UNKNOWN;
        }
        private int AddChain(pdb crystal)
        {
            int ChainIndex = crystal.PDB_CHAINS;
            crystal.chains[ChainIndex] = new Chain();
            crystal.chains[ChainIndex].residues = new Residue[PROTEIN_RESIDUE_CHUNK];
            crystal.chains[ChainIndex].SEQRES = new char[PROTEIN_RESIDUE_CHUNK];
            CHAINS_RESIDUE_SIZE[ChainIndex] = PROTEIN_RESIDUE_CHUNK;

            CHAINS_ATOM_SIZE[ChainIndex] = ATOM_CHUNK;
            crystal.chains[ChainIndex].atoms = new Atom[CHAINS_ATOM_SIZE[ChainIndex]];
            crystal.chains[ChainIndex].CHAIN_ATOMS = 0;
            crystal.PDB_CHAINS++;
            if (crystal.PDB_CHAINS >= crystal.chains.Count())
            {
                int newsize = crystal.chains.Count() + PDB_ITEMS_CHUNK;
                Array.Resize(ref crystal.chains, newsize);
                Array.Resize(ref CHAINS_RESIDUE_SIZE, newsize);
                Array.Resize(ref CHAINS_ATOM_SIZE, newsize);
            }
            return ChainIndex;
        }
        private int AddRNA(pdb crystal)
        {
            int RNAIndex = crystal.PDB_RNAS;
            crystal.rnas[RNAIndex] = new RNA();
            crystal.rnas[RNAIndex].residues = new Residue[PROTEIN_RESIDUE_CHUNK];
            crystal.rnas[RNAIndex].SEQRES = new char[PROTEIN_RESIDUE_CHUNK];
            CHAINS_RESIDUE_SIZE[RNAIndex] = PROTEIN_RESIDUE_CHUNK;

            RNA_ATOM_SIZE[RNAIndex] = ATOM_CHUNK;
            crystal.rnas[RNAIndex].atoms = new Atom[RNA_ATOM_SIZE[RNAIndex]];
            crystal.rnas[RNAIndex].RNA_ATOMS = 0;
            crystal.PDB_RNAS++;
            if (crystal.PDB_RNAS >= crystal.rnas.Count())
            {
                int newsize = crystal.rnas.Count() + PDB_ITEMS_CHUNK;
                Array.Resize(ref crystal.rnas, newsize);
            }
            return RNAIndex;
        }
        private int AddDNA(pdb crystal)
        {
            int DNAIndex = crystal.PDB_DNAS;
            crystal.dnas[DNAIndex] = new DNA();
            crystal.dnas[DNAIndex].residues = new Residue[PROTEIN_RESIDUE_CHUNK];
            crystal.dnas[DNAIndex].SEQRES = new char[PROTEIN_RESIDUE_CHUNK];
            CHAINS_RESIDUE_SIZE[DNAIndex] = PROTEIN_RESIDUE_CHUNK;

            DNA_ATOM_SIZE[DNAIndex] = ATOM_CHUNK;
            crystal.dnas[DNAIndex].atoms = new Atom[DNA_ATOM_SIZE[DNAIndex]];
            crystal.dnas[DNAIndex].DNA_ATOMS = 0;
            crystal.PDB_DNAS++;
            if (crystal.PDB_DNAS >= crystal.dnas.Count())
            {
                int newsize = crystal.dnas.Count() + PDB_ITEMS_CHUNK;
                Array.Resize(ref crystal.dnas, newsize);
            }
            return DNAIndex;
        }
        private void AddChainResidue(pdb crystal, int CurrentResidueIndex, int ResidueIndex, int ChainIndex, string AA3)
        {
            int InfiniteLoopProtection = 0;
            while (ResidueIndex > CHAINS_RESIDUE_SIZE[ChainIndex] - 1)
            {
                CHAINS_RESIDUE_SIZE[ChainIndex] += PROTEIN_RESIDUE_CHUNK;
                Array.Resize(ref crystal.chains[ChainIndex].residues, CHAINS_RESIDUE_SIZE[ChainIndex]);
                Array.Resize(ref crystal.chains[ChainIndex].SEQRES, CHAINS_RESIDUE_SIZE[ChainIndex]);
                InfiniteLoopProtection++;
                if (InfiniteLoopProtection > 10)
                    break;
            }
            int AAType = pdb.AA.GetAA3Type(AA3);
            char aa1 = pdb.AA.GetAA1(AA3);
            crystal.chains[ChainIndex].SEQRES[ResidueIndex] = aa1;
            crystal.chains[ChainIndex].CHAIN_RESIDUES = CurrentResidueIndex + 1;
            crystal.chains[ChainIndex].residues[ResidueIndex] = new Residue(AAType, aa1);
            crystal.chains[ChainIndex].residues[ResidueIndex].offset = crystal.chains[ChainIndex].CHAIN_ATOMS;
        }
        private void AddRNAResidue(pdb crystal, int CurrentResidueIndex, int ResidueIndex, int RNAIndex, string AA1)
        {
            int InfiniteLoopProtection = 0;
            while (ResidueIndex > RNA_RESIDUE_SIZE[RNAIndex] - 1)
            {
                RNA_RESIDUE_SIZE[RNAIndex] += PROTEIN_RESIDUE_CHUNK;
                Array.Resize(ref crystal.rnas[RNAIndex].residues, RNA_RESIDUE_SIZE[RNAIndex]);
                Array.Resize(ref crystal.rnas[RNAIndex].SEQRES, RNA_RESIDUE_SIZE[RNAIndex]);
                InfiniteLoopProtection++;
                if (InfiniteLoopProtection > 10)
                    break;
            }
            int AAType = pdb.RNB.GetAA1Type(AA1);
            //char aa1 = pdb.NB.GetAA1(AA2);
            char aa1 = AA1[0];
            crystal.rnas[RNAIndex].SEQRES[ResidueIndex] = aa1;
            crystal.rnas[RNAIndex].RNA_RESIDUES = CurrentResidueIndex + 1;
            crystal.rnas[RNAIndex].residues[ResidueIndex] = new Residue(AAType, aa1);
            crystal.rnas[RNAIndex].residues[ResidueIndex].offset = crystal.rnas[RNAIndex].RNA_ATOMS;
        }
        private void AddDNAResidue(pdb crystal, int CurrentResidueIndex, int ResidueIndex, int DNAIndex, string AA2)
        {
            int InfiniteLoopProtection = 0;
            while (ResidueIndex > DNA_RESIDUE_SIZE[DNAIndex] - 1)
            {
                DNA_RESIDUE_SIZE[DNAIndex] += PROTEIN_RESIDUE_CHUNK;
                Array.Resize(ref crystal.dnas[DNAIndex].residues, DNA_RESIDUE_SIZE[DNAIndex]);
                Array.Resize(ref crystal.dnas[DNAIndex].SEQRES, DNA_RESIDUE_SIZE[DNAIndex]);
                InfiniteLoopProtection++;
                if (InfiniteLoopProtection > 10)
                    break;
            }
            int AAType = pdb.DNB.GetAA2Type(AA2);
            char aa1 = pdb.DNB.GetAA1(AA2);
            crystal.dnas[DNAIndex].SEQRES[ResidueIndex] = aa1;
            crystal.dnas[DNAIndex].DNA_RESIDUES = CurrentResidueIndex + 1;
            crystal.dnas[DNAIndex].residues[ResidueIndex] = new Residue(AAType, aa1);
            crystal.dnas[DNAIndex].residues[ResidueIndex].offset = crystal.dnas[DNAIndex].DNA_ATOMS;
        }
        private void AddChainAtom(pdb crystal, int ChainIndex, Atom atom)
        {
            crystal.chains[ChainIndex].atoms.SetValue(atom, crystal.chains[ChainIndex].CHAIN_ATOMS);
            crystal.chains[ChainIndex].CHAIN_ATOMS++;
            if (crystal.chains[ChainIndex].CHAIN_ATOMS >= CHAINS_ATOM_SIZE[ChainIndex])
            {
                CHAINS_ATOM_SIZE[ChainIndex] += ATOM_CHUNK;
                Array.Resize(ref crystal.chains[ChainIndex].atoms, CHAINS_ATOM_SIZE[ChainIndex]);                       
            }
        }
        private void AddRNAAtom(pdb crystal, int RNAIndex, Atom atom)
        {
            crystal.rnas[RNAIndex].atoms.SetValue(atom, crystal.rnas[RNAIndex].RNA_ATOMS);
            crystal.rnas[RNAIndex].RNA_ATOMS++;
            if (crystal.rnas[RNAIndex].RNA_ATOMS >= RNA_ATOM_SIZE[RNAIndex])
            {
                RNA_ATOM_SIZE[RNAIndex] += ATOM_CHUNK;
                Array.Resize(ref crystal.rnas[RNAIndex].atoms, RNA_ATOM_SIZE[RNAIndex]);
            }
        }
        private void AddDNAAtom(pdb crystal, int DNAIndex, Atom atom)
        {
            crystal.dnas[DNAIndex].atoms.SetValue(atom, crystal.dnas[DNAIndex].DNA_ATOMS);
            crystal.dnas[DNAIndex].DNA_ATOMS++;
            if (crystal.dnas[DNAIndex].DNA_ATOMS >= DNA_ATOM_SIZE[DNAIndex])
            {
                DNA_ATOM_SIZE[DNAIndex] += ATOM_CHUNK;
                Array.Resize(ref crystal.dnas[DNAIndex].atoms, DNA_ATOM_SIZE[DNAIndex]);
            }
        }
    }
}
