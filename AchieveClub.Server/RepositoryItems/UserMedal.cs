using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchieveClub.Server.RepositoryItems
{
    public class UserMedal
    {
     
            public int Id { get; set; }
            public int UserRefId { get; set; }
            public int MedalRefId { get; set; }
        
    }
}
