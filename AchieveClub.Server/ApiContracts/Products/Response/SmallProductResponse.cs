namespace AchieveClub.Server.ApiContracts.Products.Response;

public record SmallProductResponse(int Id, string Type, string Title, int Price, List<SmallVariantResponse> Variants);