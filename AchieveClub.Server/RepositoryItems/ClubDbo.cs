using System.ComponentModel.DataAnnotations.Schema;

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

        public ClubState ToState(int avgXp)
        {
            return new ClubState(Id, Title, LogoURL, avgXp);
        }
    }

    public record ClubState(int Id, string Title, string LogoURL, int avgXp);
}