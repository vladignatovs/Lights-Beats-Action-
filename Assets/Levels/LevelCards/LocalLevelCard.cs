using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for the setup of the LocalLevelCard and the population of its data
/// </summary>
public class LocalLevelCard : MonoBehaviour, ILevelCard {
    [SerializeField] Button _playButton;
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _audioText;
    [SerializeField] TMP_Text _bpmText;
    [SerializeField] Button _publishButton;
    [SerializeField] Button _exportButton;
    [SerializeField] Button _deleteButton;

    public void Setup(LevelMetadata metadata, ILevelCardCallbacks callbacks) {
        _playButton.onClick.AddListener(async () => await callbacks.OnPlayLevel(metadata));
        _nameText.text = metadata.name;
        _audioText.text = metadata.audioPath;
        _bpmText.text = metadata.bpm.ToString();
        _publishButton.onClick.AddListener(() => callbacks.OnPublishLevel(metadata.id));
        _exportButton.onClick.AddListener(async () => await callbacks.OnExportLevel(metadata.id));
        _deleteButton.onClick.AddListener(() => callbacks.OnDeleteLocalLevel(metadata.id));
        // TODO: find a quick way to go to editor, aka set statenamemanager.level and beatamount
    }
}