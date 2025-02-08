namespace AchieveClub.Server.ApiContracts.Products.Response;

public record ProductResponse(int Id, string Type, string Title, string Details, int Price, List<VariantResponse> Variants);