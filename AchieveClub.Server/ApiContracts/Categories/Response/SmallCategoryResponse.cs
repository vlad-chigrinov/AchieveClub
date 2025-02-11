namespace AchieveClub.Server.ApiContracts.Categories.Response;

public record SmallCategoryResponse(int Id, string Title, string? Color, DateTime? StartDate, DateTime? EndDate);