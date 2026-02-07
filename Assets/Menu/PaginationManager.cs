using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PaginationManager {
    int _currentPage = 0;
    int _pageSize = 10; // TODO: make it configurable in the settings
    private Dictionary<int, List<LevelMetadata>> _serverCache = new();
    private Dictionary<int, List<LevelMetadata>> _localCache = new();
    private int _localTotalCount = 0;
    private int _serverTotalCount = 0;
    LocalLevelManager _localManager;
    ServerLevelManager _serverManager;
    public PaginationManager(
        LocalLevelManager local,
        ServerLevelManager server
    ) {
        _localManager = local;
        _serverManager = server;
    }

    public void GoToNextPage() {
        GoToPage(_currentPage + 1);
    }

    public void GoToPreviousPage() {
        GoToPage(_currentPage - 1);
    }

    public void GoToPage(int page) {
        _currentPage = Math.Max(0, page); // cant be less than zero
    }

    public async Task<List<LevelMetadata>> GetServerLevelsPage() {
        if (_serverCache.ContainsKey(_currentPage)) {
            return _serverCache[_currentPage];
        }
        
        int offset = _currentPage * _pageSize;
        var (pageData, totalCount) = await _serverManager.LazyLoadLevels(offset, _pageSize);
        _serverTotalCount = totalCount;
        _serverCache[_currentPage] = pageData;
        return pageData;
    }

    public async Task<List<LevelMetadata>> GetLocalLevelsPage() {
        if (_localCache.ContainsKey(_currentPage)) {
            return _localCache[_currentPage];
        }
        
        int offset = _currentPage * _pageSize;
        var (pageData, totalCount) = await _localManager.LazyLoadLevels(offset, _pageSize);
        _localTotalCount = totalCount;
        _localCache[_currentPage] = pageData;
        return pageData;
    }
    
    public int GetLocalTotalPages() {
        return (_localTotalCount + _pageSize - 1) / _pageSize;
    }
    
    public int GetServerTotalPages() {
        return (_serverTotalCount + _pageSize - 1) / _pageSize;
    }
    
    public int CurrentPage => _currentPage;
}