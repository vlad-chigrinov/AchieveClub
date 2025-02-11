using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("Variants")]
public class VariantDbo
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public int Quantity { get; set; }
    public int? DefaultPhotoId { get; set; }
    [ForeignKey(nameof(DefaultPhotoId))]
    public ProductPhotoDbo? DefaultPhoto { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public ProductDbo? Product { get; set; }
    
    public List<ProductPhotoDbo>? ProductPhotos { get; set; }
}