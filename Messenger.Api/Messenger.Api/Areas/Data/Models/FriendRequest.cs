namespace Messenger.Api.Areas.Data.Models
{
    public record FriendRequest(int SenderId, int RecipientId, RequestStatus Status, DateTime? SentDate);
}
