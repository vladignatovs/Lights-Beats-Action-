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
    [SerializeField] Slider _baseSlider;
    [SerializeField] Image _completionFill;
    [SerializeField] Slider _completionSlider;
    [SerializeField] Image _accuracyFill;
    [SerializeField] TMP_Text _attemptCount;
    public void Setup(LevelMetadata metadata, ILevelCardCallbacks callbacks, Completion completion = null) {
        _playButton.onClick.AddListener(async () => await callbacks.OnPlayLevel(metadata));
        _nameText.text = metadata.name;
        _creatorButton.onClick.AddListener(() => callbacks.OnCreatorOverview(metadata));
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

        // TODO: dedupe
        if(completion != null) {
            float completionPercent = Mathf.Clamp01(completion.percentage);
            float accuracyPercent = Mathf.Clamp01(completion.accuracy);

            _attemptCount.text = completion.attempts.ToString();
            _baseSlider.value = completionPercent;
            _completionFill.fillAmount = completionPercent;
            _completionSlider.value = accuracyPercent;
            _accuracyFill.fillAmount = accuracyPercent;
        }
    }
}
