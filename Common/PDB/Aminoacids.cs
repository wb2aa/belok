using System;
using System.Text;

namespace Belok.Common.PDB
{
    public class Aminoacids
    {
        public int[,,] BB = new int[20,2,12];
        public int NAA = 20; //number of aminoacids
        public int NBB = 12; //max number of backbones
        public int NATM_MAX = 14;
        public char[] AA1 ={
           'A',  'R',  'N',  'D',  'C',  'Q',  'E',  'G',  'H',  'I',
		   'L',  'K',  'M',  'F',  'P',  'S',  'T',  'W',  'Y',  'V'};
        public int NHPB = 8;
        public static char[] HYDROPHOBIC = {'I', 'L', 'V', 'M', 'F', 'Y', 'C', 'W'};
        public static string[] AA3 = {
         "ALA","ARG","ASN","ASP","CYS","GLN","GLU","GLY","HIS","ILE",
		 "LEU","LYS","MET","PHE","PRO","SER","THR","TRP","TYR","VAL"};
        public int[] sizeList= {
             5,   11,    8,    8,    6,    9,    9,    4,   10,   8,
		     8,    9,    8,   11,    7,    6,    7,   14,   12,   7};
        public int[] bb_sizeList = {
             1,    7,    4,    4,    2,    5,    5,    0,    7,   4,
			 4,    5,    4,    8,    4,    2,    3,   12,    9,   3};

        public string [,] atomsList = new string[20, 14] {
	    {"N", "CA", "C", "O", "CB",   "",     "",    "",    "",    "",    "",    "",    "",    ""},//ALA
	    {"N", "CA", "C", "O", "CB", "CG",   "CD",  "NE",  "CZ", "NH1", "NH2",	 "",    "",    ""},//ARG
	    {"N", "CA", "C", "O", "CB", "CG",  "OD1", "ND2",    "",    "",    "",    "",    "",    ""},//ASN
	    {"N", "CA", "C", "O", "CB", "CG",  "OD1", "OD2",	"",    "",    "",    "",    "",    ""},//ASP
	    {"N", "CA", "C", "O", "CB", "SG",	 "",    "",     "",    "",    "",    "",    "",    ""},//CYS
	    {"N", "CA", "C", "O", "CB", "CG",   "CD", "OE1", "NE2",    "",    "",    "",    "",    ""},//GLN
	    {"N", "CA", "C", "O", "CB", "CG",   "CD", "OE1", "OE2",	   "",    "",    "",    "",    ""},//GLU
	    {"N", "CA", "C", "O",   "",   "",     "",    "",    "",    "",    "",    "",    "",    ""},//GLY
	    {"N", "CA", "C", "O", "CB", "CG",  "ND1", "CD2", "CE1", "NE2",    "",    "",    "",    ""},//HIS
	    {"N", "CA", "C", "O", "CB", "CG1", "CG2", "CD1",	"",    "",    "",    "",    "",    ""},//ILE
	    {"N", "CA", "C", "O", "CB", "CG",  "CD1", "CD2",	"",    "",    "",    "",    "",    ""},//LEU
	    {"N", "CA", "C", "O", "CB", "CG",   "CD",  "CE",  "NZ",    "",    "",    "",    "",    ""},//LYS
	    {"N", "CA", "C", "O", "CB", "CG",   "SD",  "CE",    "",    "",    "",    "",    "",    ""},//MET
	    {"N", "CA", "C", "O", "CB", "CG",  "CD1", "CD2", "CE1", "CE2",  "CZ",    "",    "",    ""},//PHE
	    {"N", "CA", "C", "O", "CB", "CG",   "CD",	"",     "",    "",    "",    "",    "",    ""},//PRO
	    {"N", "CA", "C", "O", "CB", "OG",     "",    "",    "",    "",    "",    "",    "",    ""},//SER
	    {"N", "CA", "C", "O", "CB", "OG1", "CG2",    "",    "",    "",    "",    "",    "",    ""},//THR
	    {"N", "CA", "C", "O", "CB", "CG",  "CD1", "CD2", "NE1", "CE2", "CE3", "CZ2", "CZ3", "CH2"},//TRP
	    {"N", "CA", "C", "O", "CB", "CG",  "CD1", "CD2", "CE1", "CE2",  "CZ",  "OH",    "",    ""},//TYR
	    {"N", "CA", "C", "O", "CB", "CG1", "CG2",	 "",    "",    "",    "",    "",    "",    ""} //VAL
        };

        /*
        "G","GLY", 4,"Glycine"		N CA C O
        "A","ALA", 5,"Alanine"		N CA C O CB
        "C","CYS", 6,"Cysteine"		N CA C O CB SG									CB-SG
        "S","SER", 6,"Serine"		N CA C O CB OG									CB-OG
        "T","THR", 7,"Threonine"	N CA C O CB OG1 CG2								CB-OG1						CB-OG2
        "P","PRO", 7,"Proline"		N CA C O CB CG  CD								CB-CG-CD-N-CA
        "V","VAL", 7,"Valine"		N CA C O CB CG1 CG2								CB-CG1						CB-CG2
        "L","LEU", 8,"Leucine"		N CA C O CB CG  CD1 CD2							CB-CG-CD1					CG-CD2
        "I","ILE", 8,"Isoleucine"	N CA C O CB CG1 CG2 CD1							CB-CG1-CD1					CB-CG2
        "N","ASN", 8,"Asparagine"	N CA C O CB CG  OD1 ND2							CB-CG-OD1					CG-ND2
        "D","ASP", 8,"Aspartic acid"N CA C O CB CG  OD1 OD2							CB-CG-OD1					CG-OD2
        "M","MET", 8,"Methionine",	N CA C O CB CG  SD  CE							CB-CG-SD-CE
        "E","GLU", 9,"Glutamic acid"N CA C O CB CG  CD  OE1 OE2						CB-CG-CD-OE1				CD-OE2
        "Q","GLN", 9,"Glutamine"	N CA C O CB CG  CD  OE1 NE2						CB-CG-CD-OE1				CD-NE2
        "K","LYS", 9,"Lysine"		N CA C O CB CG  CD  CE  NZ						CB-CG-CD-CE-NZ
        "H","HIS",10,"Histidine"	N CA C O CB CG  ND1 CD2 CE1 NE2					CB-CG-CD2-NE2-CE1-ND1-CG
        "F","PHE",11,"Phenylalanine"N CA C O CB CG  CD1 CD2 CE1 CE2 CZ				CB-CG-CD1-CE1-CZ-CE2-CD2-CG
        "R","ARG",11,"Arginine"		N CA C O CB CG  CD  NE  CZ  NH1 NH2				CB-CG-CD-NE-CZ-NH1			CZ-NH2
        "Y","TYR",12,"Tyrosine"		N CA C O CB CG  CD1 CD2 CE1 CE2 CZ  OH			CB-CG-CD1-CE1-CZ-OH			CG-CD2-CE2-CZ
        "W","TRP",14,"Tryptophan"	N CA C O CB CG  CD1 CD2 NE1 CE2 CE3 CZ2 CZ3 CH2 CB-CG-CD1-NE1-CE2-CD2-CG    CE2-CZ2-CH2-CZ3-CE3
        */

        public string[,] BACK_BONE = new string[20, 24] { //new string [20, 12, 2]
        /*ALA*/{"CA", "CB",    "",   "",    "",    "",    "",   "",    "",   "",    "",   "",    "",   "",
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*ARG*/{"CA", "CB",  "CB", "CG",  "CG",  "CD",  "CD", "NE",  "NE", "CZ",  "CZ", "NH1", "CZ", "NH2",
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""}, 
        /*ASN*/{"CA", "CB",  "CB", "CG",  "CG", "OD1", "CG", "ND2", "",   "",    "",   "",    "",   "",
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*ASP*/{"CA", "CB",  "CB", "CG",  "CG", "OD1", "CG", "OD2", "",   "",    "",   "",    "",   "", 
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*CYS*/{"CA", "CB",  "CB", "SG",    "",    "",    "",   "",    "",   "",    "",   "",    "",   "", 
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*GLN*/{"CA", "CB",  "CB", "CG",  "CG",  "CD",  "CD", "OE1", "CD", "NE2", "",   "",    "",   "", 
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*GLU*/{"CA", "CB",  "CB", "CG",  "CG",  "CD",  "CD", "OE1", "CD", "OE2", "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*GLY*/{  "",   "",    "",   "",    "",    "",    "",   "",    "",   "",    "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*HIS*/{"CA", "CB",  "CB", "CG",  "CG", "ND1", "CG", "CD2", "ND1","CE1", "CE1","NE2", "NE2","CD2",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*ILE*/{"CA", "CB",  "CB", "CG1", "CB", "CG2", "CG1","CD1", "",   "",    "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*LEU*/{"CA", "CB",  "CB", "CG",  "CG", "CD1", "CG", "CD2", "",   "",    "",   "",    "",   "", 
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*LYS*/{"CA", "CB",  "CB", "CG",  "CG",  "CD",  "CD", "CE",  "CE", "NZ",  "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*MET*/{"CA", "CB",  "CB", "CG",  "CG",  "SD",  "SD", "CE",  "",   "",    "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*PHE*/{"CA", "CB",  "CB", "CG",  "CG", "CD1", "CG", "CD2", "CD1","CE1", "CD2","CE2", "CE1","CZ",  
	           "CE2", "CZ",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*PRO*/{"CA", "CB",  "CB", "CG",  "CG",  "CD",  "CD", "N",   "",   "",    "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*SER*/{"CA", "CB",  "CB", "OG",    "",    "",    "",   "",    "",   "",    "",   "",    "",   "",  
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*THR*/{"CA", "CB",  "CB","OG1",  "CB", "CG2", "",   "",    "",   "",    "",   "",    "",   "",  
                  "",   "",    "",   "",    "",    "",    "",   "",    "",   ""},  
        /*TRP*/{"CA", "CB",  "CB", "CG",  "CG", "CD1", "CG", "CD2", "CD1","NE1", "CD2","CE2", "NE1","CE2",
               "CE2","CZ2", "CZ2","CH2", "CH2", "CZ3", "CZ3","CE3", "CE3","CD2"},
        /*TYR*/{"CA", "CB",  "CB", "CG",  "CG", "CD1", "CG", "CD2", "CD1","CE1", "CD2","CE2", "CE1","CZ",  
	           "CE2" ,"CZ",  "CZ", "OH",    "",    "",    "",   "",    "",   ""},  
        /*VAL*/{"CA", "CB",  "CB","CG1",  "CB", "CG2", "",   "",    "",   "",    "",   "",    "",   "",  
	              "",   "",    "",   "",    "",    "", "",   "",    "",   ""}
        };

        public double[,] atomsRadii = new double[20,14] {
        //   N     CA   C    O     CB
        {1.5,  2.0, 1.7, 1.4,  2.0,    0,      0,     0,     0,     0,     0,     0,     0,     0},//ALA
        {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,   1.5,   2.0,   1.5,   1.5,	  0,     0,     0},//ARG
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    1.4,   1.5,	 0,     0,     0,     0,     0,     0},//ASN
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    1.4,   1.4,	 0,     0,     0,     0,     0,     0},//ASP
	    {1.5,  2.0, 1.7, 1.4,  2.0, 1.85,	   0,     0,     0,     0,     0,     0,     0,     0},//CYS
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,   1.4,   1.5,     0,     0,     0,     0,     0},//GLN
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,   1.4,   1.4,	    0,     0,     0,     0,     0},//GLU
	    {1.5,  2.0, 1.7, 1.4,    0,    0,      0,     0,     0,     0,     0,     0,     0,     0},//GLY
	    {1.5,  2.0, 1.7, 1.4,  2.0,  1.7,    1.5,   1.7,   1.7,   1.5,     0,     0,     0,     0},//HIS
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,   2.0,	 0,     0,     0,     0,     0,     0},//ILE
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,   2.0,	 0,     0,     0,     0,     0,     0},//LEU
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,   2.0,   1.5,     0,     0,     0,     0,     0},//LYS
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,   1.85,   2.0,     0,     0,     0,     0,     0,     0},//MET
	    {1.5,  2.0, 1.7, 1.4,  2.0,  1.7,    1.7,   1.7,   1.7,   1.7,   1.7,     0,     0,     0},//PHE
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,     0,     0,     0,     0,     0,     0,     0},//PRO
	    {1.5,  2.0, 1.7, 1.4,  2.0,  1.4,      0,     0,     0,     0,     0,     0,     0,     0},//SER
	    {1.5,  2.0, 1.7, 1.4,  2.0,  1.4,    2.0,     0,	 0,     0,     0,     0,     0,     0},//THR
	    {1.5,  2.0, 1.7, 1.4,  2.0,  1.7,    1.7,   1.7,   1.5,   1.7,   1.7,   1.7,   1.7,   1.7},//TRP
	    {1.5,  2.0, 1.7, 1.4,  2.0,  1.7,    1.7,   1.7,   1.7,   1.7,   1.7,   1.4,     0,     0},//TYR
	    {1.5,  2.0, 1.7, 1.4,  2.0,  2.0,    2.0,	  0,     0,     0,     0,     0,     0,     0} //VAL
        };

        char NoneAA = '.';

        public Aminoacids()
        {
            for (int i = 0; i < NAA; i++)
            {
                int size = sizeList[i];
                int bb_size = bb_sizeList[i];
                for (int j = 0; j < bb_size; j++)
                {
                    for (int k = 0; k < size; k++)
                        if (atomsList[i,k] == BACK_BONE[i,2*j])
                        {
                            BB[i,0,j] = k;
                            break;
                        }
                    for (int k = 0; k < size; k++)
                        if (atomsList[i, k] == BACK_BONE[i,2*j+1])
                        {
                            BB[i,1,j] = k;
                            break;
                        }
                    //BB[2][j] = BACK_BONE_ORDER[i][j];
                }
            }
        }
        //public char GetNoneAA()
        //{
        //    return NoneAA;
        //}
        public char GetAA1(string aa3)
        {
            for (int i = 0; i < NAA; i++)
            {
                if (aa3 == AA3[i])
                    return AA1[i];
            }
            return NoneAA;
        }
        public int GetAA3Type(string aaa)
        {
            for (int i = 0; i < NAA; i++)
            {
                if (aaa == AA3[i])
                    return i;
            }
            return 0;
        }
        public static bool CheckName(string ResidueName)
        {
            foreach (string AA in AA3)
            {
                if (ResidueName == AA)
                    return true;
            }
            return false;
        }
        public bool IsHydrophobic(char A)
        {
            foreach(char H in HYDROPHOBIC)
                if (A == H)
                    return true;
            return false;
        }
    }
}
