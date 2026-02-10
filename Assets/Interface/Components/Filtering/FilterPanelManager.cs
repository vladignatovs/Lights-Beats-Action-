using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic class that defines the basics for every filter panel to be used in the project
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class FilterPanelManager<T> : MonoBehaviour {
    [SerializeField] protected Button _applyButton;
    [SerializeField] protected Button _clearButton;

    protected PaginationManager<T> _paginationManager;

    public void Initialize(PaginationManager<T> paginationManager) {
        _paginationManager = paginationManager;
    }

    protected abstract List<IFilter> BuildFilters();
    protected abstract void ClearInputs();

    /// <summary>
    /// Exponsed method to be manually set to the onClick of the apply button
    /// </summary>
    [UsedImplicitly]
    public async void OnApplyClicked() {
        var filters = BuildFilters();
        await _paginationManager.ApplyFilters(filters);
        TogglePanel(); // panel is already opened, toggle it again to close
    }

    /// <summary>
    /// Exponsed method to be manually set to the onClick of the clear button
    /// </summary>
    [UsedImplicitly]
    public async void OnClearClicked() {
        ClearInputs();
        await _paginationManager.ApplyFilters(new());
        TogglePanel();
    }

    /// <summary>
    /// Exponsed method to be manually set to the onClick of the close button
    /// as well as the toggle button outside of the panel
    /// </summary>
    [UsedImplicitly]
    public void TogglePanel() {
        bool newState = !gameObject.activeSelf;
        gameObject.SetActive(newState);
        Overlay.ToggleOverlay(newState);
    }
}
