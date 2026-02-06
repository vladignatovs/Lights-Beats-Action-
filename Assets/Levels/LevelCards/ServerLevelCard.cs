using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for the setup of the ServerLevelCard and the population of its data
/// </summary>
public class ServerLevelCard : MonoBehaviour, ILevelCard {
    [SerializeField] Button _playButton;
    [SerializeField] TMP_Text _nameText;
    [SerializeField] Button _creatorButton;
    [SerializeField] TMP_Text _creatorUsernameText;
    [SerializeField] TMP_Text _audioText;
    [SerializeField] TMP_Text _bpmText;
    [SerializeField] Button _importButton;
    [SerializeField] Button _deleteButton;
    public void Setup(LevelMetadata metadata, ILevelCardCallbacks callbacks) {
        _playButton.onClick.AddListener(async () => await callbacks.OnPlayLevel(metadata));
        _nameText.text = metadata.name;
        // TODO: creator button
        // _creatorButton.onClick.AddListener(async () => await callbacks.OnCreatorOverview(metadata));
        _creatorUsernameText.text = metadata.creatorUsername;
        _audioText.text = metadata.audioPath;
        _bpmText.text = metadata.bpm.ToString();
        if(metadata.creatorId.HasValue && SupabaseManager.Instance.Client.Auth.CurrentUser.Id == metadata.creatorId.Value.ToString()) {
            _importButton.onClick.AddListener(async () => await callbacks.OnImportLevel(metadata.serverId.Value));
            _deleteButton.onClick.AddListener(() => callbacks.OnDeleteServerLevel(metadata.serverId.Value));
        } else {
            _importButton.gameObject.SetActive(false);
            _deleteButton.gameObject.SetActive(false);
        }
    }
}