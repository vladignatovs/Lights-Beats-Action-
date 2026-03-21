using System.Collections.Generic;
using UnityEngine;

public class UserLoader : MonoBehaviour, IUserCardCallbacks {
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _userCard;
    [SerializeField] UserPaginationManager _paginationManager;
    [SerializeField] UserFilterPanel _filterPanel;
    [SerializeField] ServerSectionManager _serverSectionManager;

    UserPageManager _userPageManager;

    void Awake() {
        _userPageManager = new();
        _paginationManager.Initialize(_userPageManager);
        _filterPanel.Initialize(_paginationManager);
    }

    void OnEnable() {
        _paginationManager.OnPageLoaded += OnPageLoaded;
        _serverSectionManager.OnSectionChanged += HandleSectionChanged;

        if (_serverSectionManager.CurrentSection == ServerSection.User) {
            _ = _paginationManager.GoToPage(0);
        }
    }

    void OnDisable() {
        _paginationManager.OnPageLoaded -= OnPageLoaded;
        _serverSectionManager.OnSectionChanged -= HandleSectionChanged;
    }

    async void HandleSectionChanged(ServerSection section) {
        if (section != ServerSection.User) return;
        await _paginationManager.GoToPage(0);
    }

    void OnPageLoaded(List<UserMetadata> metadatas) {
        RenderUserCards(metadatas);
    }

    void RenderUserCards(List<UserMetadata> metadatas) {
        for (int i = _contentTransform.childCount - 1; i >= 0; i--) {
            Destroy(_contentTransform.GetChild(i).gameObject);
        }

        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_userCard, _contentTransform);
            if (!cardObject.TryGetComponent<IUserCard>(out var card)) continue;
            card.Setup(metadata, this);
        }
    }
}
