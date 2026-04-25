using System;
using System.Threading.Tasks;

/// <summary>
/// Single source of truth for all actions exposed by user cards.
/// </summary>
public interface IUserCardCallbacks : ICallbacks {
    UserFriendRequestState GetFriendRequestState(System.Guid userId);
    Task<bool> OnToggleFriendRequest(System.Guid userId, bool hasOutgoingRequest);
    Task<bool> OnRespondToFriendRequest(System.Guid userId, bool accept);
    Task OnRemoveFriend(System.Guid userId);
    Task<bool> OnToggleBlockUser(Guid userId, bool isBlocked);
    void OnMessageUser(UserMetadata metadata);
}
