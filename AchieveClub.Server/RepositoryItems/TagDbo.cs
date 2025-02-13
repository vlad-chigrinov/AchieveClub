using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("Tags")]
public class TagDbo
{
    public int Id { get; set; }
    [MaxLength(50)]
    public required string Title { get; set; }
    [MaxLength(8)]
    public required string Color { get; set; }
}