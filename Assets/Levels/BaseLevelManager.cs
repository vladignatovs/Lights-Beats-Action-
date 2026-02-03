using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class BaseLevelManager<TId> {
    /// <summary>
    /// Lazily load a list of levels that expose minimal information about the level
    /// </summary>
    /// <returns>A list of lazily loaded levels that contain minimal amount of information about the level</returns>
    public abstract Task<List<LevelMetadata>> LazyLoadLevels();

    /// <summary>
    /// Eagerly load a full level with all the data needed to be processed during the game
    /// </summary>
    /// <param name="id">The id of the level to be loaded</param>
    /// <returns>A full Level object</returns>
    public abstract Task<Level> LoadLevel(TId id);
}