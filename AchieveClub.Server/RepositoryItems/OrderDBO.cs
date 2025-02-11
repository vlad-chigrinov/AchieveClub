using System.ComponentModel.DataAnnotations.Schema;

namespace AchieveClub.Server.RepositoryItems;

[Table("Orders")]
public class OrderDBO
{
    public int Id { get; set; }
    public int Price { get; set; }
    public DateTime OrderDate { get; set; }
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public UserDbo? User { get; set; }
    public int DeliveryStatusId { get; set; }
    [ForeignKey(nameof(DeliveryStatusId))]
    public DeliveryStatusDBO? DeliveryStatus { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public ProductDbo? Product { get; set; }
    public int VariantId { get; set; }
    [ForeignKey(nameof(VariantId))]
    public VariantDbo? Variant { get; set; }
}