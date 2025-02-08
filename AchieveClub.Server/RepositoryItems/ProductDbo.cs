using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("Products")]
public class ProductDbo
{
    [Key]
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Name { get; set; }
    public required string Details { get; set; }
    public int Price { get; set; }
    public int? DefaultVariantId { get; set; }
    [ForeignKey(nameof(DefaultVariantId))]
    public VariantDbo? DefaultVariant { get; set; }
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public CategoryDbo? Category { get; set; }

    public List<VariantDbo>? Variants { get; set; }
}