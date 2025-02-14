namespace AchieveClub.Server.ApiContracts.CompletedAchievements.Response;

public record SmallCompletedAchievementResponse(int AchieveId, int CompletionCount, long? NextTryUnix);