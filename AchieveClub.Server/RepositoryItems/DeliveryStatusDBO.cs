using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("DeliveryStatuses")]
public class DeliveryStatusDBO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Color { get; set; }
}