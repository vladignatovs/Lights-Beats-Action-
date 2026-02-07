using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for the setup of the LocalLevelCard and the population of its data
/// </summary>
public class LocalLevelCard : MonoBehaviour, ILevelCard {
    [SerializeField] Button _playButton;
    [SerializeField] TMP_InputField _nameInput;
    [SerializeField] TMP_Dropdown _audioDropdown;
    [SerializeField] AudioDropdownManager _audioDropdownManager;
    [SerializeField] TMP_InputField _bpmInput;
    [SerializeField] Button _publishButton;
    [SerializeField] Button _exportButton;
    [SerializeField] Button _deleteButton;

    public void Setup(LevelMetadata metadata, ILevelCardCallbacks callbacks) {
        var id = metadata.id;
        _playButton.onClick.AddListener(async () => await callbacks.OnPlayLevel(metadata));

        _nameInput.text = metadata.name;
        _nameInput.onEndEdit.AddListener(async value => await callbacks.OnUpdateLevelName(id, value));
        
        _audioDropdownManager.EnsurePopulated();

        var index = _audioDropdown.options.FindIndex(o => o.text == metadata.audioPath);
        if (index < 0) index = 0; 

        _audioDropdown.SetValueWithoutNotify(index);
        _audioDropdown.RefreshShownValue();

        _audioDropdown.onValueChanged.AddListener(async index => {
            var audioPath = _audioDropdown.options[index].text;
            await callbacks.OnUpdateLevelAudioPath(id, audioPath);
        });

        _bpmInput.text = metadata.bpm.ToString();
        _bpmInput.onEndEdit.AddListener(async value => await callbacks.OnUpdateLevelBpm(id, value));

        _publishButton.onClick.AddListener(() => callbacks.OnPublishLevel(id));
        _exportButton.onClick.AddListener(async () => await callbacks.OnExportLevel(id));
        _deleteButton.onClick.AddListener(() => callbacks.OnDeleteLocalLevel(id));
        // TODO: find a quick way to go to editor, aka set statenamemanager.level and beatamount
    }
}