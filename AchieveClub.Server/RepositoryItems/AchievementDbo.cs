using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchieveClubServer.Data.DTO
{
    [Table("Achievements")]
    public class AchievementDbo
    {
        public int Id { get; set; }
        public int Xp { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LogoURL { get; set; }
        public bool IsMultiple { get; set; }

        public AchievementState ToState(int ratio)
        {
            return new AchievementState(Id, Xp, Title, Description, LogoURL, ratio);
        }
    }
    public record AchievementState(
        int Id,
        int Xp,
        string Title,
        string Description,
        string LogoURL,
        int CompletionRatio
        );
}