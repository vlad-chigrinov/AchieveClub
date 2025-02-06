 using Microsoft.Extensions.Caching.Distributed;

namespace AchieveClub.Server.Services
{
    public class EmailProofService(ILogger<EmailProofService> logger, IDistributedCache cache)
    {
        public int GenerateProofCode(string emailAdress)
        {
            logger.LogDebug($"GenerateProofCode for emailAdress:{emailAdress}");
            var random = new Random();
            int proofCode = random.Next(1000, 9999);
            StoreProofCode(emailAdress, proofCode);
            return proofCode;
        }

        public bool Contains(string emailAddress)
        {
            return string.IsNullOrEmpty(cache.GetString(emailAddress)) == false;
        }

        private void StoreProofCode(string emailAddress, int proofCode)
        {
            cache.SetString(emailAddress, proofCode.ToString(), new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)});
        }

        public bool ValidateProofCode(string emailAddress, int userCode)
        {
            var proofCode = cache.GetString(emailAddress);
            if (proofCode == null)
                return false;

            return userCode == int.Parse(proofCode);
        }

        public void DeleteProofCode(string emailAddress)
        {
            cache.Remove(emailAddress);
        }
    }
}