using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class QuickLevelPaginationClear : MonoBehaviour {
    [SerializeField] LevelPaginationManager _paginationManager;
    [SerializeField] ServerLevelFilterPanel _filterPanel;

    void Awake() {
        _paginationManager.OnFiltersChanged += Refresh;
        Refresh(_paginationManager.HasCurrentFilters);
    }

    void OnDestroy() {
        _paginationManager.OnFiltersChanged -= Refresh;
    }

    void Refresh(bool hasCurrentFilters) {
        gameObject.SetActive(hasCurrentFilters);
    }

    [UsedImplicitly]
    public async void OnClearButtonClicked() {
        _filterPanel.ClearInputs();
        await _paginationManager.ApplyFilters(new()); 
    }
}
