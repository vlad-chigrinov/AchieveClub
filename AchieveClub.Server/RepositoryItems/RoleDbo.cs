using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems
{
    [Table("Roles")]
    public class RoleDbo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<UserDbo> Users { get; set; }
    }
}
