using System;
using System.Text;

/*
"A", 4,"Adenine"	P O1P O2P O5* C5* C4* O4* C3* O3* C2* O2* C1* N9 C8 N7 C5 C6 N6 N1 C2 N3 C4
"G", 5,"Guanine"	P O1P O2P O5* C5* C4* O4* C3* O3* C2* O2* C1* N9 C8 N7 C5 C6 O6 N1 C2 N2 N3 C4
"C", 6,"Cytosine"	P O1P O2P O5* C5* C4* O4* C3* O3* C2* O2* C1* N1 C2 O2 N3 C4 N4 C5 C6
"U", 6,"Uracil"		P O1P O2P O5* C5* C4* O4* C3* O3* C2* O2* C1* N1 C2 O2 N3 C4 O4 C5 C6 
*/

/*
"A", 4,"Adenine"	P OP1 OP2 O5' C5' C4' O4' C3' O3' C2' O2' C1' N9 C8 N7 C5 C6 N6 N1 C2 N3 C4
"G", 5,"Guanine"	P OP1 OP2 O5' C5' C4' O4' C3' O3' C2' O2' C1' N9 C8 N7 C5 C6 O6 N1 C2 N2 N3 C4
"C", 6,"Cytosine"	P OP1 OP2 O5' C5' C4' O4' C3' O3' C2' O2' C1' N1 C2 O2 N3 C4 N4 C5 C6
"U", 6,"Uracil"		P OP1 OP2 O5' C5' C4' O4' C3' O3' C2' O2' C1' N1 C2 O2 N3 C4 O4 C5 C6 
*/

namespace Belok.Common.PDB
{
    public class  DNA_Nucleobases
    {
        public int[, ,] BB = new int[4, 2, 24];
        public int NNB = 4; //number of bases
        public int NBB = 24; //max number of backbones
        public int NATM_MAX = 22;
        public int NATM_BB = 11;
        public char[] AA1 = { 'A', 'G', 'C', 'T' };
        public static string[] AA2 = { "DA", "DG", "DC", "DT" };
        public int[] sizeList = { 21, 22, 19, 20 };
        public int[] bb_sizeList = { 23, 24, 20, 21 };

        public string[,] atomsList = new string[4, 22] {
        {"P", "OP1","OP2","O5'","C5'","C4'","O4'","C3'","O3'","C2'","C1'", //B-bone
	    "N9", "C8", "N7", "C5", "C6", "N6", "N1", "C2", "N3", "C4", ""},         //A
        {"P", "OP1","OP2","O5'","C5'","C4'","O4'","C3'","O3'","C2'","C1'", //B-bone
	    "N9", "C8", "N7", "C5", "C6", "O6", "N1", "C2", "N2", "N3", "C4"},       //G
        {"P", "OP1","OP2","O5'","C5'","C4'","O4'","C3'","O3'","C2'","C1'", //B-bone
	    "N1", "C2", "O2", "N3", "C4", "N4", "C5", "C6", "",   "",   ""},         //C
        {"P", "OP1","OP2","O5'","C5'","C4'","O4'","C3'","O3'","C2'","C1'", //B-bone
	    "N1", "C2", "O2", "N3", "C4", "O4", "C5", "C7", "C6", "",   ""}          //Thymine
        };

        public string[,] BACK_BONE = new string[4, 48] {
        {"P",  "OP1", "P",   "OP2", "P",   "O5'", "O5'", "C5'", "C5'", "C4'", "C4'", "O4'",//A
        "O4'", "C1'", "C1'", "C2'", "C2'", "C3'", "C3'", "C4'", "C3'", "O3'",
        "C1'", "N9",  "N9",  "C8",  "C8",  "N7",  "N7",  "C5",  "C5",  "C6",  "C6",  "N6", 
        "C6",  "N1",  "N1",  "C2",  "C2",  "N3",  "N3",  "C4",  "C4",  "C5",  "C4",  "N9",   "",""},

        {"P",  "OP1", "P",   "OP2", "P",   "O5'", "O5'", "C5'", "C5'", "C4'", "C4'", "O4'",//G
        "O4'", "C1'", "C1'", "C2'", "C2'", "C3'", "C3'", "C4'", "C3'", "O3'",
        "C1'", "N9",  "N9",  "C8",  "C8",  "N7",  "N7",  "C5",  "C5",  "C6",  "C6",  "O6",
        "C6",  "N1",  "N1",  "C2",  "C2",  "N2",  "C2",  "N3",  "N3",  "C4",  "C4",  "C5",  "C4",  "N9"},

        {"P",  "OP1", "P",   "OP2", "P",   "O5'", "O5'", "C5'", "C5'", "C4'", "C4'", "O4'",//C
        "O4'", "C1'", "C1'", "C2'", "C2'", "C3'", "C3'", "C4'", "C3'", "O3'", "C1'", "N1",
        "N1",  "C6",  "C6",  "C5",  "C5",  "C4",  "C4",  "N4",  "C4",  "N3",  "N3",  "C2",
        "C2",  "O2",  "C2",  "N1",  "",    "",    "",    "",    "",    "",    "",    ""},   

        {"P",  "OP1", "P",   "OP2", "P",   "O5'", "O5'", "C5'", "C5'", "C4'", "C4'", "O4'",//T
        "O4'", "C1'", "C1'", "C2'", "C2'", "C3'", "C3'", "C4'", "C3'", "O3'", "C1'", "N1",
        "N1",  "C6",  "C6",  "C5",  "C5",  "C4",  "C4",  "O4",  "C4",  "N3",  "N3",  "C2",
        "C2",  "O2",  "C2",  "N1",  "C5",  "C7",    "",    "",    "",    "",    "",    ""}
        };
        //Nucleic acid atomic radii for PB calculations (N. Banavali)
        public double[,] atomsRadii = new double[4, 22] {
        //  P,  OP1,  OP2,  O5',  C5',  C4',   O4',   C3',   O3',   C2',   C1',
        {2.35, 1.49, 1.49, 1.65, 2.57, 2.50,  1.55,  2.73,  1.65,  2.70,  2.57,
         2.13, 2.12, 1.69, 2.12, 2.12, 2.17,  2.15,  2.15,  1.69,  2.12,    0},//A
        {2.35, 1.49, 1.49, 1.65, 2.57, 2.50,  1.55,  2.73,  1.65,  2.70,  2.57,
         2.13, 2.12, 1.69, 2.12, 2.12, 1.55,  2.15,  2.15,  2.12,  1.69,  2.12},//G
	    {2.35, 1.49, 1.49, 1.65, 2.57, 2.50,  1.55,  2.73,  1.65,  2.70,  2.57,
         2.20, 2.04, 1.60, 1.68, 2.12, 2.08,  2.25,  2.25,     0,     0,     0},//C
	    {2.35, 1.49, 1.49, 1.65, 2.57, 2.50,  1.55,  2.73,  1.65,  2.70,  2.57,
         2.20, 2.04, 1.60, 1.68, 2.12, 1.60,  2.25,  2.25,  2.25,     0,     0},//T
        };
        char NoneNB = '.';
        public DNA_Nucleobases()
        {
            for (int i = 0; i < NNB; i++)
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
        public char GetAA1(string aa2)
        {
            for (int i = 0; i < NNB; i++)
            {
                if (aa2 == AA2[i])
                    return AA1[i];
            }
            return NoneNB;
        }
        public int GetAA1Type(string aa1)
        {
            for (int i = 0; i < NNB; i++)
            {
                if (aa1[0] == AA1[i])
                    return i;
            }
            return 0;
        }
        public int GetAA2Type(string aa2)
        {
            for (int i = 0; i < NNB; i++)
            {
                if (aa2 == AA2[i])
                    return i;
            }
            return 0;
        }
        public static bool CheckName(string ResidueName)
        {
            foreach (string NB in AA2)
            {
                if (ResidueName == NB)
                    return true;
            }
            return false;
        }
    }
}
