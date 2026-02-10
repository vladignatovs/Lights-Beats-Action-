using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for the setup of the OfficialLevelCard and the population of its data
/// </summary>
public class OfficialLevelCard : MonoBehaviour, ILevelCard {
    [SerializeField] Button _playButton;
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _audioText;
    [SerializeField] TMP_Text _bpmText;

    public void Setup(LevelMetadata metadata, ILevelCardCallbacks callbacks) {
        _playButton.onClick.AddListener(async () => await callbacks.OnPlayLevel(metadata));
        _nameText.text = metadata.name;
        _audioText.text = metadata.audioPath;
        _bpmText.text = metadata.bpm.ToString();
    }
}