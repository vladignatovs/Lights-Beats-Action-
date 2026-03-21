/// <summary>
/// General interface describing a User Card.
/// </summary>
public interface IUserCard : ICard {
    void Setup(UserMetadata metadata, IUserCardCallbacks callbacks);
}
