using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchieveClub.Server.RepositoryItems
{
    [Table("Clubs")]
    public class ClubDbo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string LogoURL { get; set; }
        public List<UserDbo> Users { get; set; }
    }
}