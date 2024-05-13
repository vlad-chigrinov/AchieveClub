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

        public SmallClubState ToSmallState(int avgXp)
        {
            return new SmallClubState(Id, Title, LogoURL, avgXp);
        }

        public ClubState ToState(int avgXp, List<UserState> users)
        {
            return new ClubState(Id, Title, Description, Address, LogoURL, avgXp, users);
        }
    }

    public record SmallClubState(int Id, string Title, string LogoURL, int AvgXp);
    public record ClubState(int Id, string Title, string Description, string Address, string LogoURL, int AvgXp, List<UserState> Users);
}