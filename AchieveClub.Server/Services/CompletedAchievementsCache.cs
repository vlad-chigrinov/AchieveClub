using AchieveClub.Server.RepositoryItems;
using AchieveClubServer.Data.DTO;
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
        private readonly ApplicationContext _db = db;
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<CompletedAchievementsCache> _logger = logger;

        public List<CompletedAchievementState> GetByUserId(int userId)
        {
            var completedStatesString = _cache.GetString($"{nameof(CompletedAchievementState)}:{userId}");
            if (completedStatesString == null)
            {
                return UpdateByUserId(userId);
            }
            else
            {
                var completedStates = DeserializeAchievements(completedStatesString);
                if (completedStates == null)
                    throw new Exception("Error on deserializing completed achievements from cache!");
                _logger.LogInformation($"Get {nameof(CompletedAchievementState)}:{userId} from cache");
                return completedStates;
            }
        }

        public List<CompletedAchievementState> UpdateByUserId(int userId)
        {
            var completedStates = RecalculateUser(userId);
            _cache.SetString(
                $"{nameof(CompletedAchievementState)}:{userId}",
                SerializeAchievements(completedStates));
            _logger.LogInformation($"Store {nameof(CompletedAchievementState)}:{userId} in cache");
            return completedStates;
        }

        private List<CompletedAchievementState> RecalculateUser(int userId)
        {
            var completedDtos = _db.CompletedAchievements.Include(ca => ca.Achievement).Where(ca => ca.UserRefId == userId).ToList();

            if (completedDtos.Count == 0)
                return new();

            var completedStates = new List<CompletedAchievementState>();

            foreach (var completedDto in completedDtos)
            {
                if (completedDto.Achievement.IsMultiple)
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
