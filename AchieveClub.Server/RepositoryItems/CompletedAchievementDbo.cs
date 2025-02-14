using System.ComponentModel.DataAnnotations.Schema;
using AchieveClub.Server.ApiContracts.CompletedAchievements.Response;

namespace AchieveClub.Server.RepositoryItems;

[Table("CompletedAchievements")]
public class CompletedAchievementDbo
{
    public int Id { get; set; }
    public int SupervisorRefId { get; set; }       
    public int UserRefId { get; set; }  
    public int AchieveRefId { get; set; }
    public DateTime DateOfCompletion { get; set; }

    [ForeignKey(nameof(SupervisorRefId))]
    public UserDbo? Supervisor { get; set; }

    [ForeignKey(nameof(UserRefId))]
    public UserDbo? User { get; set; }

    [ForeignKey(nameof(AchieveRefId))]
    public AchievementDbo? Achievement { get; set; }

    public DetailedCompletedAchievementResponce MapToDetailed()
    {
        return new DetailedCompletedAchievementResponce
        {
            AchieveId = this.AchieveRefId,
            SupervisorId = this.SupervisorRefId,
            SupervisorName = this.Supervisor?.FirstName + " " + this.Supervisor?.LastName,
            CompletionDate = this.DateOfCompletion
        };
    }
}