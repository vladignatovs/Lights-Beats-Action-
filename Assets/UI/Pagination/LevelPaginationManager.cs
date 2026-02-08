public class LevelPaginationManager : PaginationManager<LevelMetadata> {
    /// <summary>
    /// Method allowing persiting the internal current page tracked by a pagination manager
    /// </summary>
    public void PersistCurrentPage() {
        StateNameManager.LastLevelPage = _currentPage;
    }
}
