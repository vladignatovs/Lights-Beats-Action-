using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest.Interfaces;
using static Supabase.Postgrest.Constants;

public class ChangeLogManager : DataManager {
    public ChangeLogManager(Client client) : base(client) {
    }

    public async Task<(List<ChangeLogMetadata> items, int totalCount)> LazyLoadChangeLogs(int offset, int limit) {
        var query = _client
            .From<ServerChangeLog>()
            .Select("id,news_id,admin_id,action,created_at")
            .Order(x => x.CreatedAt, Ordering.Descending)
            .Order(x => x.Id, Ordering.Descending);

        IPostgrestTable<ServerChangeLog> countQuery = _client.From<ServerChangeLog>();

        var getTask = query.Range(offset, offset + limit - 1).Get();
        var countTask = countQuery.Count(CountType.Exact);

        await Task.WhenAll(getTask, countTask);

        var response = await getTask;
        int totalCount = await countTask;

        var items = (response.Models ?? new List<ServerChangeLog>())
            .Select(ToLocalChangeLogMetadata)
            .ToList();

        return (items, totalCount);
    }

    static ChangeLogMetadata ToLocalChangeLogMetadata(ServerChangeLog changeLog) {
        return new ChangeLogMetadata {
            id = changeLog.Id,
            newsId = changeLog.NewsId,
            adminId = changeLog.AdminId,
            action = changeLog.Action,
            createdAt = changeLog.CreatedAt,
        };
    }
}
