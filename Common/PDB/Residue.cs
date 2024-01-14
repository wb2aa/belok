using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belok.Common.PDB
{
    [Serializable]
    public class Residue
    {
        public int offset; //first index in Atom[]
        public int type; //index in AA1, AA3
        public char AA1;
        public bool complete;

        public Residue(int Type, char aa1)
        {
            offset = 0;
            type = Type;
            AA1 = aa1;
            complete = false;
        }
    }
}
