using System;
using System.Threading.Tasks;

/// <summary>
/// Single source of truth for all actions exposed by user cards.
/// </summary>
public interface IUserCardCallbacks : ICallbacks {
    Task<bool> OnToggleBlockUser(Guid userId, bool isBlocked);
}
