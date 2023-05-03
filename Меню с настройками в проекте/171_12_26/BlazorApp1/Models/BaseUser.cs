using BlazorApp1.Services;

namespace BlazorApp1.Models
{
    public abstract class BaseUser : AbstractHistory
    {
        public string CreateRandomPass()
        {
            Random random = new Random();
            string password = random.Next(1000, 9999).ToString();

            return password;
        }

        public string GetHash(string inputString)
        {
            return Utils.SecretHasher.Hash(inputString);
        }
    }
}
