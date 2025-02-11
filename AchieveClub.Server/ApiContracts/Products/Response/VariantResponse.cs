namespace AchieveClub.Server.ApiContracts.Products.Response;

public record VariantResponse(int Id, bool Default, string Title, string Color, bool Available, List<VariantPhotoResponse> Photos);