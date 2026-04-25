using System.Collections.Generic;
using UnityEngine;

public class ChangeLogLoader : MonoBehaviour {
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _changeLogCard;
    [SerializeField] ChangeLogPaginationManager _paginationManager;
    [SerializeField] NewsSectionManager _sectionManager;

    ChangeLogPageManager _changeLogPageManager;

    void Awake() {
        _changeLogPageManager = new();
        _paginationManager.Initialize(_changeLogPageManager);
    }

    void OnEnable() {
        _paginationManager.OnPageLoaded += OnPageLoaded;
        _sectionManager.OnSectionChanged += HandleSectionChanged;

        if (_sectionManager.CurrentSection == NewsSection.Logs) {
            HandleSectionChanged(NewsSection.Logs);
        }
    }

    void OnDisable() {
        _paginationManager.OnPageLoaded -= OnPageLoaded;
        _sectionManager.OnSectionChanged -= HandleSectionChanged;
    }

    async void HandleSectionChanged(NewsSection section) {
        if (section != NewsSection.Logs || !SupabaseManager.Instance.User.IsAdmin) {
            return;
        }

        await _paginationManager.GoToPage(0);
    }

    void OnPageLoaded(List<ChangeLogMetadata> metadatas) {
        for (int i = _contentTransform.childCount - 1; i >= 0; i--) {
            Destroy(_contentTransform.GetChild(i).gameObject);
        }

        if (metadatas.Count == 0) {
            _paginationManager.ShowEmptyState();
            return;
        }

        _paginationManager.HideEmptyState();

        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_changeLogCard, _contentTransform);
            if (!cardObject.TryGetComponent<ChangeLogCard>(out var card)) {
                continue;
            }

            card.Setup(metadata);
        }
    }
}
