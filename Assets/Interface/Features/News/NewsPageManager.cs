using System.Collections.Generic;
using System.Threading.Tasks;

public class NewsPageManager : IPageProvider<NewsMetadata> {
    public async Task<(List<NewsMetadata> items, int totalCount)> LoadPage(int offset, int pageSize, List<IFilter> filters = null) {
        List<IDataFilter<ServerNewsMetadata>> dataFilters = null;
        if (filters != null) {
            dataFilters = new List<IDataFilter<ServerNewsMetadata>>();
            foreach (var filter in filters) {
                if (filter is IDataFilter<ServerNewsMetadata> dataFilter) {
                    dataFilters.Add(dataFilter);
                }
            }
        }

        return await SupabaseManager.Instance.News.LazyLoadNews(offset, pageSize, dataFilters);
    }
}
