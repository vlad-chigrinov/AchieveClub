using AchieveClub.Server.RepositoryItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AchieveClub.Server.Services
{
    public class CompletedAchievementsCache(
        ApplicationContext db,
        IDistributedCache cache,
        ILogger<CompletedAchievementsCache> logger)
    {
        public List<CompletedAchievementState> GetByUserId(int userId)
        {
            var completedStatesString = cache.GetString($"{nameof(CompletedAchievementState)}:{userId}");
            if (completedStatesString == null)
            {
                logger.LogDebug($"Update value in cache: {nameof(CompletedAchievementState)}:{userId}");
                return UpdateByUserId(userId);
            }
            else
            {
                var completedStates = DeserializeAchievements(completedStatesString);
                if (completedStates == null)
                {
                    var error = $"Error on deserializing {nameof(CompletedAchievementState)}:{userId} from cache!";
                    logger.LogError(error);
                    throw new Exception(error);
                }
                logger.LogDebug($"Get {nameof(CompletedAchievementState)}:{userId} from cache");
                return completedStates;
            }
        }

        public List<CompletedAchievementState> UpdateByUserId(int userId)
        {
            var completedStates = RecalculateUser(userId);
            cache.SetString(
                $"{nameof(CompletedAchievementState)}:{userId}",
                SerializeAchievements(completedStates));
            logger.LogInformation($"Store {nameof(CompletedAchievementState)}:{userId} in cache");
            return completedStates;
        }

        private List<CompletedAchievementState> RecalculateUser(int userId)
        {
            var completedDtos = db.CompletedAchievements.Include(ca => ca.Achievement).Where(ca => ca.UserRefId == userId).ToList();

            if (completedDtos.Count == 0)
                return new();

            var completedStates = new List<CompletedAchievementState>();

            foreach (var completedDto in completedDtos)
            {
                if (completedDto.Achievement!.IsMultiple)
                {
                    if (completedStates.Any(ca => ca.AchieveId == completedDto.AchieveRefId))
                        continue;

                    var completionCount = completedDtos.Count(ca => ca.AchieveRefId == completedDto.AchieveRefId);
                    completedStates.Add(new(completedDto.AchieveRefId, completionCount));
                }
                else
                {
                    completedStates.Add(new(completedDto.AchieveRefId, 0));
                }
            }

            return completedStates;
        }

        private string SerializeAchievements(List<CompletedAchievementState> completedAchievementStates)
        {
            return JsonSerializer.Serialize(completedAchievementStates);
        }

        private List<CompletedAchievementState>? DeserializeAchievements(string completedAchievementStates)
        {
            return JsonSerializer.Deserialize<List<CompletedAchievementState>>(completedAchievementStates);
        }
    }
}
