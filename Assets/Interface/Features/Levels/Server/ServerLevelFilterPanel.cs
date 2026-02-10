using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerLevelFilterPanel : FilterPanelManager<LevelMetadata> {
    [Header("Name Filter")]
    [SerializeField] TMP_InputField _nameInput;
    [SerializeField] Toggle _nameEqualsToggle;
    [SerializeField] Toggle _nameStartsWithToggle;
    [SerializeField] Toggle _nameContainsToggle;

    [Header("Other Filters")]
    [SerializeField] Toggle _ownedToggle;
    [SerializeField] TMP_InputField _bpmMinInput;
    [SerializeField] TMP_InputField _bpmMaxInput;
    [SerializeField] AudioDropdownManager _audioDropdown;

    protected override List<IFilter> BuildFilters() {
        var filters = new List<IDataFilter<ServerLevelMetadata>>();

        if (_ownedToggle.isOn) {
            var userId = SupabaseManager.Instance.Client.Auth.CurrentUser?.Id;
            if (!string.IsNullOrEmpty(userId)) {
                filters.Add(new OwnedByFilter(userId));
            }
        }

        // Name filter - check which mode is selected
        if (!string.IsNullOrWhiteSpace(_nameInput.text)) {
            if (_nameStartsWithToggle.isOn) {
                filters.Add(new NameStartsWithFilter(_nameInput.text));
            } else if (_nameContainsToggle.isOn) {
                filters.Add(new NameContainsFilter(_nameInput.text));
            } else if (_nameEqualsToggle.isOn) {
                filters.Add(new NameEqualsFilter(_nameInput.text));
            }
        }

        bool hasMinBpm = _bpmMinInput.text.FloatTryParse(out float minBpm);
        bool hasMaxBpm = _bpmMaxInput.text.FloatTryParse(out float maxBpm);
        
        if (hasMinBpm && hasMaxBpm) {
            filters.Add(new BpmBetweenFilter(minBpm, maxBpm));
        } else if (hasMinBpm) {
            filters.Add(new BpmBetweenFilter(minBpm, float.MaxValue));
        } else if (hasMaxBpm) {
            filters.Add(new BpmBetweenFilter(0, maxBpm));
        }

        _audioDropdown.EnsurePopulated();
        var dropdown = _audioDropdown.GetComponent<TMP_Dropdown>();
        if (dropdown != null && dropdown.value > 0) { // 0 is "None", TODO: might remove None from dropdown values
            string selectedAudio = dropdown.options[dropdown.value].text;
            filters.Add(new AudioPathEqualsFilter(selectedAudio));
        }

        var genericFilters = new List<IFilter>();
        foreach (var filter in filters) {
            genericFilters.Add(filter);
        }
        return genericFilters;
    }

    protected override void ClearInputs() {
        _nameInput.text = "";
        _bpmMinInput.text = "";
        _bpmMaxInput.text = "";
        
        var dropdown = _audioDropdown.GetComponent<TMP_Dropdown>();
        if (dropdown != null) {
            dropdown.value = 0;
        }
    }
}
