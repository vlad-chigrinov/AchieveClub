using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class EmailProofService(IDistributedCache cache)
    {
        private readonly IDistributedCache _cache = cache;

        public int GenerateProofCode(string emailAdress)
        {
            var random = new Random();
            int proofCode = random.Next(1000, 9999);
            StoreProofCode(emailAdress, proofCode);
            return proofCode;
        }

        public bool Contains(string emailAddress)
        {
            return string.IsNullOrEmpty(_cache.GetString(emailAddress)) == false;
        }

        private void StoreProofCode(string emailAddress, int proofCode)
        {
            _cache.SetString(emailAddress, proofCode.ToString(), new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)});
        }

        public bool ValidateProofCode(string emailAddress, int userCode)
        {
            var proofCode = _cache.GetString(emailAddress);
            if (proofCode == null)
                return false;

            return userCode == int.Parse(proofCode);
        }

        public void DeleteProofCode(string emailAddress)
        {
            _cache.Remove(emailAddress);
        }
    }
}