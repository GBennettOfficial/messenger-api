namespace Messenger.Api.Areas.Cryptography.Models
{
    public record HashedPassword(string Hash, string Salt);
}
