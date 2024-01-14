using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belok.Common.PDB
{
    public class Domain
    {
        public string NAME;      //one character chain name - A
        public int DOMAIN_BITS;  //number of sequence fragments
        public DomainBit[] bits; //chain sequence - SSQRSVARMDG

        public Domain()
        {
            DOMAIN_BITS = 0;
        }
        //604-636,811-860
        public Domain(string chain, string domain)
        {
            NAME = chain;
            char[] comma = { ',' };
            string[] domains = domain.Split(comma, StringSplitOptions.RemoveEmptyEntries);
            DOMAIN_BITS = domains.Length;
            bits = new DomainBit[DOMAIN_BITS];
            for (int i = 0; i < DOMAIN_BITS; i++)
            {
                string[] residues = domains[i].Split('-');
                bits[i] = new DomainBit();
                bits[i].start = Convert.ToInt32(residues[0]);
                bits[i].end = Convert.ToInt32(residues[1]);
            }
        }
        public Domain(string chain, List<string> domain)
        {
            NAME = chain;
            string[] domains = domain.ToArray();
            DOMAIN_BITS = domains.Length;
            bits = new DomainBit[DOMAIN_BITS];
            for (int i = 0; i < DOMAIN_BITS; i++)
            {
                string[] residues = domains[i].Split('-');
                bits[i] = new DomainBit();
                bits[i].start = Convert.ToInt32(residues[0]);
                bits[i].end = Convert.ToInt32(residues[1]);
            }
        }
        public bool InDomain(int iRes)
        {
            bool inDomain = false;
            for (int i = 0; i < DOMAIN_BITS; i++)
            {
                if ((iRes >= bits[i].start) && (iRes <= bits[i].end))
                {
                    inDomain = true;
                    break;
                }
            }
            return inDomain;
        }
        private int GetResSize()
        {
            int size = 0;
            for (int i = 0; i < DOMAIN_BITS; i++)
                size += bits[i].end - bits[i].start + 1;
            return size;
        }
        public int[] GetResList()
        {
            int[] resList = new int[GetResSize()];
            int k = 0;
            for (int i = 0; i < DOMAIN_BITS; i++)
            {
                for (int j = bits[i].start; j <= bits[i].end; j++)
                {
                    resList[k] = j;
                    k++;
                }

            }
            return resList;
        }
    }
    public class DomainBit
    {
        public int start;
        public int end;

        public DomainBit()
        {
            start = -1;
            end = -1;
        }
    }
}
