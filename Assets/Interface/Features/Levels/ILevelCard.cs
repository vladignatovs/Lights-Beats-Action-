/// <summary>
/// General interface describing a Level Card, the key component as
/// it connects the main menu to the gameplay
/// </summary>
public interface ILevelCard {
    void Setup(LevelMetadata metadata, ILevelCardCallbacks levelCardCallbacks);
}