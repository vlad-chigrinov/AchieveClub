using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("ProductPhotos")]
public class ProductPhotoDbo
{
    [Key]
    public int Id { get; set; }
    public required string Url { get; set; }
    public int VariantId { get; set; }
    [ForeignKey(nameof(VariantId))]
    public VariantDbo? Variant { get; set; }
}