﻿namespace AchieveClub.Server.ApiContracts.CompletedAchievements.Response
{
    public class DetailedCompletedAchievementResponce
    {
        public required int AchieveId { get; set; }
        public required int SupervisorId { get; set; }
        public required string SupervisorName { get; set; }
        public required DateTime CompletionDate { get; set; }
    }
}
