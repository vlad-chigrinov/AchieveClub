using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems
{
    [Table("CompletedAchievements")]
    public class CompletedAchievementDbo
    {
        public int Id { get; set; }
        public int SupervisorRefId { get; set; }       
        public int UserRefId { get; set; }  
        public int AchieveRefId { get; set; }
        public DateTime DateOfCompletion { get; set; }
    }
    public record CompletedAchievementState(int AchieveId);
}
