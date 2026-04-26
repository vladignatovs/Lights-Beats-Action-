using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsPanelManager : MonoBehaviour {
    [SerializeField] GameObject _panel;
    [SerializeField] Slider _itemsPerPageSlider;
    [SerializeField] TMP_Text _itemsPerPageValueText;
    [SerializeField] Slider _volumeSlider;
    [SerializeField] TMP_Text _volumeValueText;

    void OnEnable() {
        GameSettingsManager.OnSettingsChanged += HandleSettingsChanged;
        SyncSliders();
    }

    void OnDisable() {
        GameSettingsManager.OnSettingsChanged -= HandleSettingsChanged;
    }

    [UsedImplicitly]
    public void TogglePanel() {
        bool shouldOpen = !_panel.activeSelf;
        _panel.SetActive(shouldOpen);
        Overlay.ToggleOverlay(shouldOpen);

        if (shouldOpen) {
            SyncSliders();
        }
    }

    [UsedImplicitly]
    public void SetItemsPerPage(float value) {
        _itemsPerPageValueText.text = Mathf.RoundToInt(value).ToString();
        GameSettingsManager.SetItemsPerPage(Mathf.RoundToInt(value));
    }

    [UsedImplicitly]
    public void SetVolume(float value) {
        _volumeValueText.text = Mathf.RoundToInt(value).ToString();
        GameSettingsManager.SetVolume(Mathf.RoundToInt(value));
    }

    void HandleSettingsChanged() {
        SyncSliders();
    }

    void SyncSliders() {
        _itemsPerPageSlider.SetValueWithoutNotify(GameSettingsManager.ItemsPerPage);
        _volumeSlider.SetValueWithoutNotify(GameSettingsManager.Volume);
        _itemsPerPageValueText.text = GameSettingsManager.ItemsPerPage.ToString();
        _volumeValueText.text = GameSettingsManager.Volume.ToString();
    }
}
