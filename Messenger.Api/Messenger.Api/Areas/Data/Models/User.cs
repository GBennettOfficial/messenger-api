namespace Messenger.Api.Areas.Data.Models
{
    public record User(int Id,
                       string Username,
                       string UserCode,
                       string FirstName,
                       string LastName,
                       string Email,
                       string? Phone,
                       string PasswordHash,
                       string PasswordSalt,
                       string? Picture = null,
                       DateTime? RegistrationDate = null,
                       DateTime? LastLoginDate = null,
                       bool? IsEmailVerified = null,
                       bool? IsPhoneVerified = null);
}
