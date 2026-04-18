using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class NewsLoader : MonoBehaviour, INewsCardCallbacks {
    [SerializeField] GameObject _newsPanel;
    [SerializeField] Transform _contentTransform;
    [SerializeField] GameObject _newsCard;
    [SerializeField] CreateNewsManager _createNewsManager;
    [SerializeField] NewsPaginationManager _paginationManager;
    [SerializeField] NewsFilterPanel _filterPanel;
    [SerializeField] NewsView _newsView;
    [SerializeField] NewsSectionManager _sectionManager;
    [SerializeField] GameObject _toggleCreateNewsButton;

    NewsPageManager _newsPageManager;

    void Awake() {
        _newsPageManager = new();
        _paginationManager.Initialize(_newsPageManager);
        _filterPanel.Initialize(_paginationManager);
    }

    void OnEnable() {
        _paginationManager.OnPageLoaded += OnPageLoaded;

        _ = _paginationManager.GoToPage(0);
    }

    void OnDisable() {
        _paginationManager.OnPageLoaded -= OnPageLoaded;
    }

    [UsedImplicitly]
    public async void ToggleNewsPanel() {
        bool shouldOpen = !_newsPanel.activeSelf;
        Overlay.ToggleOverlay(shouldOpen);
        _newsPanel.SetActive(shouldOpen);
        _sectionManager.ToListSection();
        _toggleCreateNewsButton.SetActive(SupabaseManager.Instance.User.IsAdmin);

        if (shouldOpen) {
            await _paginationManager.GoToPage(0);
        }
    }

    void OnPageLoaded(List<NewsMetadata> metadatas) {
        RenderNewsCards(metadatas ?? new List<NewsMetadata>());
    }

    void RenderNewsCards(List<NewsMetadata> metadatas) {
        for (int i = _contentTransform.childCount - 1; i >= 0; i--) {
            Destroy(_contentTransform.GetChild(i).gameObject);
        }

        foreach (var metadata in metadatas) {
            var cardObject = Instantiate(_newsCard, _contentTransform);
            if (!cardObject.TryGetComponent<INewsCard>(out var card)) {
                continue;
            }

            card.Setup(metadata, this);
        }
    }

    public async Task OnOpenNews(long newsId) {
        await _newsView.Show(newsId);
    }

    public async Task OnEditNews(long newsId) {
        var news = await SupabaseManager.Instance.News.LoadNewsById(newsId);
        if (news == null) {
            return;
        }

        _createNewsManager.BeginEdit(news);
    }

    public async Task OnDeleteNews(long newsId) {
        await SupabaseManager.Instance.News.DeleteNews(newsId);
        await _paginationManager.ReloadPage();
    }
}
