using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("Roles")]
public class RoleDbo
{
    public required  int Id { get; set; }
    [MaxLength(50)] public required string Title { get; set; }
    public List<UserDbo> Users { get; set; } = [];
}