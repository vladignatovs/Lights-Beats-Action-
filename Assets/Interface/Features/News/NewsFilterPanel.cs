using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsFilterPanel : FilterPanelManager<NewsMetadata> {
    [Header("Title Filter")]
    [SerializeField] TMP_InputField _titleInput;
    [SerializeField] Toggle _titleEqualsToggle;
    [SerializeField] Toggle _titleStartsWithToggle;
    [SerializeField] Toggle _titleContainsToggle;

    [Header("Category Filter")]
    [SerializeField] NewsCategoryDropdownManager _categoryDropdown;

    protected override List<IFilter> BuildFilters() {
        var filters = new List<IDataFilter<ServerNewsMetadata>>();

        if (!string.IsNullOrWhiteSpace(_titleInput.text)) {
            if (_titleStartsWithToggle.isOn) {
                filters.Add(new NewsTitleStartsWithFilter(_titleInput.text));
            } else if (_titleContainsToggle.isOn) {
                filters.Add(new NewsTitleContainsFilter(_titleInput.text));
            } else if (_titleEqualsToggle.isOn) {
                filters.Add(new NewsTitleEqualsFilter(_titleInput.text));
            }
        }

        _categoryDropdown.EnsurePopulated();
        var dropdown = _categoryDropdown.GetComponent<TMP_Dropdown>();
        if (dropdown.value > 0) {
            var selectedCategory = dropdown.options[dropdown.value].text;
            filters.Add(new NewsCategoryEqualsFilter(selectedCategory));
        }

        var genericFilters = new List<IFilter>();
        foreach (var filter in filters) {
            genericFilters.Add(filter);
        }
        return genericFilters;
    }

    protected override void ClearInputs() {
        _titleInput.text = "";

        var dropdown = _categoryDropdown.GetComponent<TMP_Dropdown>();
        dropdown.value = 0;
    }

    [UsedImplicitly]
    public new async void OnApplyClicked() {
        var filters = BuildFilters();
        await _paginationManager.ApplyFilters(filters);
    }

    [UsedImplicitly]
    public new async void OnClearClicked() {
        ClearInputs();
        await _paginationManager.ApplyFilters(new());
    }
}
