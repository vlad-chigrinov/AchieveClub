﻿namespace AchieveClub.Server.Auth;

public record EmailSettings
{
    public string ApiKey { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
}