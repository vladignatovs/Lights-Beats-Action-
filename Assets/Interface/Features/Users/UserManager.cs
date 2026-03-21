using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserPageManager : IPageProvider<UserMetadata> {
    public async Task<(List<UserMetadata> items, int totalCount)> LoadPage(int offset, int pageSize, List<IFilter> filters = null) {
        List<IDataFilter<ServerUserMetadata>> dataFilters = null;
        if (filters != null) {
            dataFilters = new List<IDataFilter<ServerUserMetadata>>();
            foreach (var filter in filters) {
                if (filter is IDataFilter<ServerUserMetadata> dataFilter) {
                    dataFilters.Add(dataFilter);
                }
            }
        }

        return await SupabaseManager.Instance.User.LazyLoadUsers(offset, pageSize, dataFilters);
    }
}
