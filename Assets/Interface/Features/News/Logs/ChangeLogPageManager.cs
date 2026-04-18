using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChangeLogPageManager : IPageProvider<ChangeLogMetadata> {
    public async Task<(List<ChangeLogMetadata> items, int totalCount)> LoadPage(int offset, int pageSize, List<IFilter> filters = null) {
        var page = await SupabaseManager.Instance.ChangeLog.LazyLoadChangeLogs(offset, pageSize);

        var adminIds = page.items
            .Select(item => item.adminId)
            .Distinct()
            .ToList();

        var userTasks = adminIds.ToDictionary(id => id, LoadUserById);
        await Task.WhenAll(userTasks.Values);

        foreach (var item in page.items) {
            var user = userTasks[item.adminId].Result;
            item.adminName = string.IsNullOrWhiteSpace(user?.username)
                ? item.adminId.ToString()
                : user.username;
        }

        return page;
    }

    static async Task<UserMetadata> LoadUserById(Guid userId) {
        return await SupabaseManager.Instance.User.LoadUserById(userId);
    }
}
