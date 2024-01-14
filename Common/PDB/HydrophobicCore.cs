using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belok.Common.PDB
{
    class HydrophobicCore
    {
        private double R_MAX = 4.8;
        private int ATM_START = 5;
        private int MIN_CONTACT = 3;
        private int MAX_CONTACT_MAP = 20;
        private int N_HYDROPHOBIC = 9;
        private char[] HYDROPHOBIC = {'A', 'I', 'L', 'V', 'M', 'F', 'Y', 'C', 'W'};
        private int n_select_min = 10;

        public Residue[] CoreResidues;
        public Domain CoreDomain;

        public HydrophobicCore()
        {
        }
        public void GetHydrophobicCoreResidues(Chain chain, Domain domain = null)
        {
            int[] res_list;
            int[,] contact_map = GetContactMap(chain, null, out res_list);
            int n_residues = chain.CHAIN_RESIDUES;
            //number of residues in hydrophobic core
            int n_list = 0;
            for (int ir = 0; ir < n_residues; ir++)
            {
                if (res_list[ir] >= MIN_CONTACT)
                    n_list++;
            }
            int[] id_list = new int[n_list];
            //maximum number of contacts for one residue
            int max_contact = 1;
            for (int ir = 0; ir < n_residues; ir++)
            {
                if (res_list[ir] >= MIN_CONTACT)
                {
                    if (max_contact < res_list[ir]) max_contact = res_list[ir];
                }
            }
            //histogram of contacts per one residue for domain
            int[] n_res_contact;
            if (max_contact > MIN_CONTACT)
                n_res_contact = new int[max_contact + 1];
            else
                n_res_contact = new int[MIN_CONTACT + 1];
            for (int i = 0; i <= max_contact; i++)
                n_res_contact[i] = 0;
            for (int ir = 0; ir < n_residues; ir++)
            {
                if (domain != null && !domain.InDomain(ir))
                    continue;
                if (res_list[ir] >= 0)
                {
                    if (res_list[ir] >= max_contact)
                    {
                        ;
                    }
                    n_res_contact[res_list[ir]]++;
                }
            }
            //select contact level to get more then <n_select_min> residues
            int select_contact = MIN_CONTACT, n_select = 0;
            if (n_select_min > 0)
            {
                for (int i = max_contact; i > 0; i--)
                {
                    n_select += n_res_contact[i];
                    if (n_select >= n_select_min)
                    {
                        select_contact = i;
                        break;
                    }
                }
            }
            if (select_contact < MIN_CONTACT)
                select_contact = MIN_CONTACT;
            //final list
            int k = 0;
            for (int ir = 0; ir < n_residues; ir++)
            {
                if (domain != null && !domain.InDomain(ir))
                    continue;
                if (res_list[ir] >= select_contact)
                {
                    id_list[k] = ir; k++;
                }
            }
            //n_list = k;
            ////clustering
            //int[] cluster = new int[n_residues];
            //int n_new, current_cluster = -1; bool find;
            //while(true)
            //{
            //    find = false;
            //    for(int ir = 0; ir < n_residues; ir++)
            //    {
            //        if(res_list[ir] < MIN_CONTACT) continue;
            //        if(cluster[ir] == -1)
            //        {
            //            current_cluster++;
            //            cluster[ir] = current_cluster;
            //            find = true;
            //            break;
            //        }
            //    }
            //    if(!find) break;
            //    while(true)
            //    {
            //        n_new = 0;
            //        for(int ir = 0; ir < n_residues; ir++)
            //        {
            //            if(cluster[ir] == current_cluster)
            //            {
            //                for(int i = 0; i < res_list[ir]; i++)
            //                {
            //                    k = contact_map[i, ir];
            //                    if(res_list[k] < MIN_CONTACT)
            //                        continue;
            //                    if(cluster[k] == -1)
            //                    {
            //                        cluster[k] = current_cluster;
            //                        n_new++;
            //                    }
            //                }
            //            }
            //        }
            //        if(n_new == 0)
            //            break;
            //    }
            //    //if(n_new == 0) break;
            //}
            //output
            string domainResidues = "";
            int bitStart = -1;
            int bitEnd = -1;
            int nRes = 0;
	        for(int ir = 0; ir < n_residues; ir++)
            {
                if (domain != null && !domain.InDomain(ir))
                    continue;
                if (res_list[ir] >= select_contact)
                {
                    nRes++;
                    if (bitStart == -1)
                    {
                        bitStart = ir;
                        bitEnd = ir;
                    }
                    else
                    {
                        if (ir == (bitEnd + 1))
                            bitEnd = ir;
                        else
                        {
                            domainResidues += Convert.ToString(bitStart) + "-" + Convert.ToString(bitEnd) + ",";
                            bitStart = ir;
                            bitEnd = ir;
                        }
                    }
                }
            }
            if (bitStart != -1)
                domainResidues += Convert.ToString(bitStart) + "-" + Convert.ToString(bitEnd);
            CoreDomain = new Domain(chain.NAME, domainResidues);
            int kRes = 0;
            CoreResidues = new Residue[nRes];
            for(int ir = 0; ir < n_residues; ir++)
            {
                if (domain != null && !domain.InDomain(ir))
                    continue;
                if (res_list[ir] >= select_contact)
                {
                    CoreResidues[kRes] = chain.residues[ir];
                    kRes++;
                }
            }
#region old
            /*
            	int i, k, ir;
	free_list();
	int *res_list = new int[n_residues];
	int **contact_map, *cluster;
	contact_map = new int*[MAX_CONTACT_MAP];
	for(i = 0; i < MAX_CONTACT_MAP; i++) contact_map[i] = new int[n_residues];
	cluster = new int[n_residues]; for(i = 0; i < n_residues; i++) cluster[i] = -1;
	GetHydroPhobic(res_list, contact_map);
//number of residues in hydrophobic core
	for(ir = 0; ir < n_residues; ir++) {if(res_list[ir] >= MIN_CONTACT) n_list++;}
	id_list = new int[n_list]; 
//maximum number of contacts for one residue
	int max_contact = 1;
	for(ir = 0; ir < n_residues; ir++) {
		if(res_list[ir] >= MIN_CONTACT) {
			if(max_contact < res_list[ir]) max_contact = res_list[ir];
		}
	}
//histogram of contacts per one residue
	int *n_res_contact;
	if(max_contact > MIN_CONTACT) n_res_contact = new int[max_contact + 1];
	else  n_res_contact = new int[MIN_CONTACT + 1];
	for(i = 0; i <= max_contact; i++) n_res_contact[i] = 0;
	for(ir = 0; ir < n_residues; ir++) {
		if(res_list[ir] >= 0) {
			if(res_list[ir] >= max_contact) {
				i = i;
			}
			n_res_contact[res_list[ir]]++;
		}
	}
//select contact level to get more then <n_select_min> residues
	int select_contact = MIN_CONTACT, n_select = 0;
	if(n_select_min > 0) {
		for(i = max_contact; i > 0; i--) {
			n_select += n_res_contact[i];
			if(n_select >= n_select_min) {select_contact = i; break;}
		}
	}
	if(select_contact < MIN_CONTACT) select_contact = MIN_CONTACT;
//final list
	k = 0;
	for(ir = 0; ir < n_residues; ir++) {
		if(res_list[ir] >= select_contact) {
			id_list[k] = ir; k++;
		}
	}
	n_list = k;
//break
	if(file_name == NULL) {
		delete n_res_contact;
		delete res_list;
		for(i = 0; i < MAX_CONTACT_MAP; i++) delete contact_map[i]; delete contact_map;
		delete cluster;
		return NULL;
	}
//clustering
	int n_new, current_cluster = -1; bool find;
	while(true) {
		find = false;
		for(ir = 0; ir < n_residues; ir++) {
			if(res_list[ir] < MIN_CONTACT) continue;
			if(cluster[ir] == -1) {current_cluster++; cluster[ir] = current_cluster; find = true; break;}
		}
		if(!find) break;
		while(true) {
			n_new = 0;
			for(ir = 0; ir < n_residues; ir++) {
				if(cluster[ir] == current_cluster) {
					for(i = 0; i < res_list[ir]; i++) {
						k = contact_map[i][ir];
						if(res_list[k] < MIN_CONTACT) continue;
						if(cluster[k] == -1) {cluster[k] = current_cluster; n_new++;}
					}
				}
			}
			if(n_new == 0) break;
		}
		//if(n_new == 0) break;
	}
//output
	int *res_list_pdb = new int[n_residues];
	char file_name_txt[255]; //char *pos;
	sprintf(file_name_txt, "HydroPhobic.txt");
	FILE *out = fopen(file_name_txt, "wt");
	for(i = 0; i <= max_contact; i++) fprintf(out, "%3d",i); fprintf(out, "\n");
	for(i = 0; i <= max_contact; i++) fprintf(out, "%3d",n_res_contact[i]); fprintf(out, "\n");
	for(ir = 0; ir < n_residues; ir++) {
		res_list_pdb[ir] = -1;
		if(res_list[ir] >= MIN_CONTACT) {
			fprintf(out, "%c %4d %4d %4d ", sequence[ir], residue_number[ir], res_list[ir], cluster[ir]);
			for(i = 0; i < res_list[ir]; i++) fprintf(out, "$%d", residue_number[contact_map[i][ir]]);
			fprintf(out, "\n");
			res_list_pdb[ir] = residue_number[ir];
		}
	}
//clusters
	int n_star;
	for(i = 0; i <= current_cluster; i++) {
		fprintf(out, "%d: ", i);
		n_new = 0; n_star = 0;
		for(ir = 0; ir < n_residues; ir++) {
			if(cluster[ir] == i) {
				if(n_new == 0) fprintf(out, "%d:", residue_number[ir]);
				if(n_star < 3) for(k = 0; k < n_star; k++) fprintf(out, "*");
				fprintf(out, "%c", sequence[ir]);
				n_new++; n_star = 0;
			}
			else {
				//if(n_star < 3) fprintf(out, "*");
				if(n_new > 0 && n_star == 3) {fprintf(out, "; "); n_new = 0;}
				 n_star++;
			}
		}
		fprintf(out, "\n");
	}
	fclose(out);
	delete n_res_contact;
	delete res_list;
	for(i = 0; i < MAX_CONTACT_MAP; i++) delete contact_map[i]; delete contact_map;
	delete cluster;
	return res_list_pdb;
*/
#endregion
            return;
        }
        private int[,] GetContactMap(Chain chain, Domain domain, out int[] res_list)
        {
            double d, dd, dd_max = R_MAX * R_MAX;
            int n_residues = chain.CHAIN_RESIDUES;
            int[,] contact_map = new int[MAX_CONTACT_MAP, n_residues];
            for (int i = 0; i < MAX_CONTACT_MAP; i++)
                for (int k = 0; k < n_residues; k++)
                    contact_map[i, k] = -1;
            res_list = new int[n_residues];
            //hydrophobic residues
            string resList;
            string sequence = chain.GetSeq(out resList, true);
            for (int ir = 0; ir < n_residues; ir++)
            {
                res_list[ir] = -1;
                if (chain.residues[ir] == null)
                    continue;
                if (domain != null && !domain.InDomain(ir))
                    continue;
                for (int i = 0; i < N_HYDROPHOBIC; i++)
                {
                    if (sequence[ir] == HYDROPHOBIC[i])
                    {
                        res_list[ir] = 0;
                    }
                }
            }
            //number of contacts
            for(int ir = 0; ir < n_residues; ir++)
            {
                if(res_list[ir] == -1)
                    continue;
                if (domain != null && !domain.InDomain(ir))
                    continue;
                Residue residue = chain.residues[ir];
                int index = residue.offset;
                for(int ir1 = ir + 1; ir1 < n_residues; ir1++)
                {
                    if(res_list[ir1] == -1)
                        continue;
                    if (domain != null && !domain.InDomain(ir))
                        continue;
                    Residue residue1 = chain.residues[ir1];
                    int index1 = residue1.offset;
                    bool find = false;
                    for(int ia = ATM_START; ia < pdb.AA.sizeList[residue.type]; ia++)
                    {
                        for(int ia1 = ATM_START; ia1 < pdb.AA.sizeList[residue1.type]; ia1++)
                        {
                            dd = 0;
                            for(int k = 0; k < 3; k++)
                            {
                                //d = pts[k][ica[ir] + ia] - pts[k][ica[ir1] + ia1];
                                d = chain.atoms[index + ia].pts[k] - chain.atoms[index1 + ia1].pts[k];
                                dd += d*d;
                            }
                            if(dd < dd_max)
                            {
                                find = true;
                                break;
                            }
                        }
                        if(find)
                            break;
                    }
                    if(find) 
                    {
                        if(res_list[ir]  < MAX_CONTACT_MAP) contact_map[res_list[ir], ir]  = ir1;
                        if(res_list[ir1] < MAX_CONTACT_MAP) contact_map[res_list[ir1], ir1] = ir;
                        res_list[ir]++; res_list[ir1]++;
                    }
                }
            }
            return contact_map;
        }
    }
}
