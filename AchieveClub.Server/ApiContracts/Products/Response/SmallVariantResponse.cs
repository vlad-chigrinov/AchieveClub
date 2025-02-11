namespace AchieveClub.Server.ApiContracts.Products.Response;

public record SmallVariantResponse(int Id, string Color, string? Photo, bool Default, bool Available);