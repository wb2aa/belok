using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//2dtg
//REMARK 350 BIOMOLECULE: 1                                                       
//REMARK 350 AUTHOR DETERMINED BIOLOGICAL UNIT: DECAMERIC 
//REMARK 350 APPLY THE FOLLOWING TO CHAINS: A, B, C, D, E                         
//REMARK 350   BIOMT1   1  1.000000  0.000000  0.000000        0.00000            
//REMARK 350   BIOMT2   1  0.000000  1.000000  0.000000        0.00000            
//REMARK 350   BIOMT3   1  0.000000  0.000000  1.000000        0.00000            
//REMARK 350   BIOMT1   2 -1.000000  0.000000  0.000000        0.00000            
//REMARK 350   BIOMT2   2  0.000000  1.000000  0.000000        0.00000            
//REMARK 350   BIOMT3   2  0.000000  0.000000 -1.000000      102.43350
//CRYST1  123.007  319.691  204.867  90.00  90.00  90.00 C 2 2 21      8          
//ORIGX1      1.000000  0.000000  0.000000        0.00000                         
//ORIGX2      0.000000  1.000000  0.000000        0.00000                         
//ORIGX3      0.000000  0.000000  1.000000        0.00000                         
//SCALE1      0.008130  0.000000  0.000000        0.00000                         
//SCALE2      0.000000  0.003128  0.000000        0.00000                         
//SCALE3      0.000000  0.000000  0.004881        0.00000  

//2i13
//REMARK 350 BIOMOLECULE: 1                                                       
//REMARK 350 AUTHOR DETERMINED BIOLOGICAL UNIT: TRIMERIC                          
//REMARK 350 APPLY THE FOLLOWING TO CHAINS: C, D, A                               
//REMARK 350   BIOMT1   1  1.000000  0.000000  0.000000        0.00000            
//REMARK 350   BIOMT2   1  0.000000  1.000000  0.000000        0.00000            
//REMARK 350   BIOMT3   1  0.000000  0.000000  1.000000        0.00000            
//REMARK 350                                                                      
//REMARK 350 BIOMOLECULE: 2                                                       
//REMARK 350 AUTHOR DETERMINED BIOLOGICAL UNIT: TRIMERIC                          
//REMARK 350 APPLY THE FOLLOWING TO CHAINS: E, F, B                               
//REMARK 350   BIOMT1   1  1.000000  0.000000  0.000000        0.00000            
//REMARK 350   BIOMT2   1  0.000000  1.000000  0.000000        0.00000            
//REMARK 350   BIOMT3   1  0.000000  0.000000  1.000000        0.00000  
//CRYST1   41.586   72.054   74.212  75.53  83.80  72.67 P 1           2          
//ORIGX1      1.000000  0.000000  0.000000        0.00000                         
//ORIGX2      0.000000  1.000000  0.000000        0.00000                         
//ORIGX3      0.000000  0.000000  1.000000        0.00000                         
//SCALE1      0.024047 -0.007503 -0.000915        0.00000                         
//SCALE2      0.000000  0.014538 -0.003426        0.00000                         
//SCALE3      0.000000  0.000000  0.013926        0.00000   
namespace Belok.Common.PDB
{
    [Serializable]
    public class Matrix
    {
        public double[,] rotate;
        public double[] translate;
        public Matrix()
        {
            rotate = new double[3,3];
            translate = new double[3];
        }
    }
    [Serializable]
    public class Biomolecule
    {
        public int nmatrix;
        public Matrix[] matrix;
        public int nchains;
        public string[] chains;
        public int[] matrixId;
        int ChainListSize;
        string[] ChainList;
        public Biomolecule()           
        {
            nmatrix = 0;
            nchains = 0;
            ChainListSize = 0;
            chains = null;
            matrixId = null;
            ChainList = null;
        }
        public void SetChainList(string[] split)
        {
            ChainListSize = split.Length - 1;
            ChainList = new string[ChainListSize];
            for (int i = 0; i < ChainListSize; i++)
            {
                ChainList[i] = split[i + 1].Trim();
            }
        }
        public void AddMatrix(string[] BIOMT)
        {
            string[] split;
            char[] space = {' '};
            Matrix newMatrix = new Matrix();
            for (int i = 0; i < 3; i++)
            {
                split = BIOMT[i].Split(space, StringSplitOptions.RemoveEmptyEntries);
                for(int k = 0; k < 3; k++)
                    newMatrix.rotate[k, i] = Convert.ToDouble(split[4 + k]);
                newMatrix.translate[i] = Convert.ToDouble(split[7]);
            }
            if (matrix == null)
                matrix = new Matrix[1];
            else
                Array.Resize(ref matrix, nmatrix + 1);
            matrix[nmatrix] = newMatrix;
            int offset = nchains;
            nchains += ChainListSize;
            if (chains == null)
            {
                chains = new string[nchains];
                matrixId = new int[nchains];
            }
            else
            {
                Array.Resize(ref chains, nchains);
                Array.Resize(ref matrixId, nchains);
            }
            for (int i = 0; i < ChainListSize; i++)
            {
                chains[offset + i] = ChainList[i];
                matrixId[offset + i] = nmatrix;
            }
            nmatrix++;
        }
    }
}
