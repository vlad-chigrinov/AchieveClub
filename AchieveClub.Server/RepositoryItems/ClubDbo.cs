using AchieveClubServer.Data.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems
{
    [Table("Clubs")]
    public class ClubDbo
    {
        public int Id { get; set; }
        public string Title_en { get; set; }
        public string Title_ru { get; set; }
        public string Title_pl { get; set; }
        public string Description_en { get; set; }
        public string Description_ru { get; set; }
        public string Description_pl { get; set; }
        public string Address_en { get; set; }
        public string Address_ru { get; set; }
        public string Address_pl { get; set; }
        public string LogoURL { get; set; }
        public List<UserDbo> Users { get; set; }

        public ClubName ToTitleState(string lang)
        {
            switch (lang)
            {
                case "en":
                    return new ClubName(Id, Title_en);
                case "ru":
                    return new ClubName(Id, Title_ru);
                case "pl":
                    return new ClubName(Id, Title_pl);
                default:
                    throw new NotImplementedException("Lang not supported!");
            }
        }

        public SmallClubState ToSmallState(int avgXp, string lang)
        {
            switch (lang)
            {
                case "en":
                    return new SmallClubState(Id, Title_en, LogoURL, avgXp);
                case "ru":
                    return new SmallClubState(Id, Title_ru, LogoURL, avgXp);
                case "pl":
                    return new SmallClubState(Id, Title_pl, LogoURL, avgXp);
                default:
                    throw new NotImplementedException("Lang not supported!");
            }
        }

        public ClubState ToState(int avgXp, List<UserState> users, string lang)
        {
            switch (lang)
            {
                case "en":
                    return new ClubState(Id, Title_en, Description_en, Address_ru, LogoURL, avgXp, users);
                case "ru":
                    return new ClubState(Id, Title_ru, Description_ru, Address_ru, LogoURL, avgXp, users);
                case "pl":
                    return new ClubState(Id, Title_en, Description_pl, Address_pl, LogoURL, avgXp, users);
                default:
                    throw new NotImplementedException("Lang not supported!");
            }
        }
    }

    public record ClubName(int Id, string Title);
    public record SmallClubState(int Id, string Title, string LogoURL, int AvgXp);
    public record ClubState(int Id, string Title, string Description, string Address, string LogoURL, int AvgXp, List<UserState> Users);
}