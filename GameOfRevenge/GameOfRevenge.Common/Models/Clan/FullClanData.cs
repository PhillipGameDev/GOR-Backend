using System.Collections.Generic;

namespace GameOfRevenge.Common.Models.Clan
{
    public class FullClanData
    {
        public ClanData Info { get; set; }
        public List<ClanMember> Members { get; set; }
        public List<int> Unions { get; set; }
        //public List<ClanInvite> Invites { get; set; }
        //public List<ClanJoinReq> JoinRequest { get; set; }
    }
}
