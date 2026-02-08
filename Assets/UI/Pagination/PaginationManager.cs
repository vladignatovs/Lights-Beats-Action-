
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic base class responsible for pagination and caching of data
/// </summary>
public abstract class PaginationManager<T> : MonoBehaviour {
    [SerializeField] Button _previousPageButton;
    [SerializeField] Button _nextPageButton;
    [SerializeField] TMP_InputField _pageInput;
    [SerializeField] TMP_Text _pageCountText;
    int _pageSize = 10; // TODO: make configurable via settings

    protected int _currentPage = 0;
    Dictionary<int, List<T>> _cache = new();
    int _totalCount = 0;
    IPageProvider<T> _pageProvider;
    List<IFilter> _currentFilters;

    public event Action<List<T>> OnPageLoaded;
    public event Action<int, int> OnPageChanged; // currentPage / totalPages

    [UsedImplicitly]
    public async void GoToPreviousPage() {
        await GoToPage(_currentPage - 1);
    }

    [UsedImplicitly]
    public async void GoToNextPage() {
        await GoToPage(_currentPage + 1);
    }

    [UsedImplicitly]
    public async void OnReloadButtonClicked() {
        await ReloadPage();
    }

    [UsedImplicitly]
    public async void OnPageInputChanged(string value) {
        if (!int.TryParse(value, out int pageNumber)) {
            UpdateUI();
            return;
        }
        
        // Convert from 1-indexed (display) to 0-indexed (internal)
        await GoToPage(pageNumber - 1);
    }

    public async Task ReloadPage() {
        _cache.Clear();
        _totalCount = 0;
        await RefreshCurrentPage();
    }

    /// <summary>
    /// Apply filters and reset to page 0
    /// </summary>
    public async Task ApplyFilters(List<IFilter> filters) {
        _currentFilters = filters;
        _cache.Clear();
        _totalCount = 0;
        await GoToPage(0);
    }

    /// <summary>
    /// Core method for the functionality of the pagination, which populates the pageProvider
    /// and allows for actual page loading
    /// </summary>
    /// <param name="pageProvider"></param>
    public void Initialize(IPageProvider<T> pageProvider) {
        _pageProvider = pageProvider;
    }

    public async Task GoToPage(int page) {
        _currentPage = Mathf.Max(0, page);
        await RefreshCurrentPage();
    }

    async Task RefreshCurrentPage() {
        var pageData = await GetCurrentPageData();
        
        if (_totalCount == 0) {
            _currentPage = 0;
        } else {
            // clamp to valid range based on actual totalCount
            int totalPages = GetTotalPages();
            if (_currentPage >= totalPages) {
                _currentPage = totalPages;
            }
            
            // if current page is empty, walk backwards to find valid page
            while ((pageData == null || pageData.Count == 0) && _currentPage > 0) {
                _currentPage--;
                pageData = await GetCurrentPageData();
            }
        }
        
        OnPageLoaded?.Invoke(pageData);
        UpdateUI();
    }

    async Task<List<T>> GetCurrentPageData() {
        if (_cache.ContainsKey(_currentPage)) {
            return _cache[_currentPage];
        }
        
        // Fetch from data provider with filters
        int offset = _currentPage * _pageSize;
        var (pageData, totalCount) = await _pageProvider.LoadPage(offset, _pageSize, _currentFilters);
        _totalCount = totalCount;
        _cache[_currentPage] = pageData;
        
        return pageData;
    }

    void UpdateUI() {
        int totalPages = GetTotalPages();
        int currentPage = _currentPage + 1; // Display as 1-indexed
        
        _pageInput.text = currentPage.ToString();
        _pageCountText.text = totalPages.ToString();
        
        // Dynamic button interactability
        _previousPageButton.interactable = _currentPage > 0;
        _nextPageButton.interactable = _currentPage < totalPages - 1;
        
        // Dynamic input field interactability (only if there are multiple pages)
        _pageInput.interactable = totalPages > 1;

        OnPageChanged?.Invoke(_currentPage, totalPages);
    }

    int GetTotalPages() {
        if (_totalCount == 0) return 0;
        return (_totalCount + _pageSize - 1) / _pageSize;
    }
}