using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AchieveClub.Server.RepositoryItems
{
    [Table("Users")]
    public class UserDbo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string? RefreshToken {  get; set; }
        public int ClubRefId { get; set; }
        [ForeignKey(nameof(ClubRefId))]
        public ClubDbo Club { get; set; }
        public int RoleRefId { get; set; }
        [ForeignKey(nameof(RoleRefId))]
        public RoleDbo Role { get; set; }

        public UserState ToUserState()
        {
            return new UserState(
                Id: this.Id,
                FirstName: this.FirstName,
                LastName: this.LastName,
                Avatar: this.Avatar,
                ClubId: this.Club.Id,
                ClubName: this.Club.Title,
                ClubLogo: this.Club.LogoURL
                );
        }
    }

    public record UserState(int Id, string FirstName, string LastName, string Avatar, int ClubId, string ClubName, string ClubLogo);
}