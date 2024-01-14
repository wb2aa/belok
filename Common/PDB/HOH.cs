using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belok.Common.PDB
{
    [Serializable]
    public class HOH
    {
        public string ChainName;    //one character chain name - A
        public int ResidueNumber;   //residue number
        public Atom atom;  
    }
}
