namespace Messenger.Api.Utilities
{
    public class RandomUserCodeGenerator
    {
        private const string _charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890123456789";
        private readonly Random _random;

        public RandomUserCodeGenerator()
        {
            int seed = Guid.NewGuid().GetHashCode();
            _random = new Random(seed);
        }

        public string GetRandomUserCode()
        {
            char[] chars = new char[12];
            for (int i = 0; i < 12; i++)
            {
                chars[i] = _charset[_random.Next(_charset.Length)];
            }
            return new string(chars);
        }
    }
}
