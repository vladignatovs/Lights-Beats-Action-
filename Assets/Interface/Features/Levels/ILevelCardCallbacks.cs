
using System;
using System.Threading.Tasks;

/// <summary>
/// Single source of truth for all actions that a card can possibly access at any point in time.
/// </summary>
public interface ILevelCardCallbacks {
    Task OnPlayLevel(LevelMetadata metadata);
    Task OnOpenEditor(LevelMetadata metadata);
    Task OnExportLevel(int id);
    void OnPublishLevel(int id);
    void OnDeleteLocalLevel(int id);
    Task OnUpdateLevelName(int id, string value);
    Task OnUpdateLevelAudioPath(int id, string value);
    Task OnUpdateLevelBpm(int id, string value);
    Task OnImportLevel(Guid id);
    void OnDeleteServerLevel(Guid id);
}