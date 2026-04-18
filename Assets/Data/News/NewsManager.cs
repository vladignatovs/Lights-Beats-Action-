using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest.Interfaces;
using static Supabase.Postgrest.Constants;

public class NewsManager : DataManager {
    public NewsManager(Client client) : base(client) {
    }

    public async Task<(List<NewsMetadata> items, int totalCount)> LazyLoadNews(
        int offset,
        int limit,
        List<IDataFilter<ServerNewsMetadata>> filters = null
    ) {
        var table = _client.From<ServerNewsMetadata>();
        var query = table.Select("id,title,category,thumbnail,created_at");

        if (filters != null) {
            foreach (var filter in filters) {
                query = filter.Apply(query);
            }
        }

        IPostgrestTable<ServerNewsMetadata> countQuery = _client.From<ServerNewsMetadata>();
        if (filters != null) {
            foreach (var filter in filters) {
                countQuery = filter.Apply(countQuery);
            }
        }

        var getTask = query.Range(offset, offset + limit - 1).Get();
        var countTask = countQuery.Count(CountType.Exact);

        await Task.WhenAll(getTask, countTask);

        var response = await getTask;
        int totalCount = await countTask;

        var items = (response.Models ?? new())
            .Select(ToLocalNewsMetadata)
            .ToList();

        return (items, totalCount);
    }

    public async Task<News> LoadNewsById(long newsId) {
        var getResponse = await _client
            .From<ServerNews>()
            .Where(x => x.Id == newsId)
            .Get();

        var model = getResponse.Model;
        return model == null ? null : ToLocalNews(model);
    }

    public async Task<News> CreateNews(string title, string content, string category, string thumbnailUrl = null) {
        var payload = new NewsInsert {
            Title = title?.Trim(),
            Content = content?.Trim(),
            Category = category,
            ThumbnailUrl = NormalizeThumbnailUrl(thumbnailUrl),
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _client
            .From<ServerNews>()
            .Insert(new ServerNews {
                Title = payload.Title,
                Content = payload.Content,
                Category = payload.Category,
                ThumbnailUrl = payload.ThumbnailUrl,
                CreatedAt = payload.CreatedAt,
            });

        var created = response.Model ?? response.Models?.FirstOrDefault();
        return created == null ? null : ToLocalNews(created);
    }

    public async Task UpdateNews(News news) {
        if (news == null) {
            throw new ArgumentNullException(nameof(news));
        }

        await _client
            .From<ServerNews>()
            .Where(x => x.Id == news.id)
            .Set(x => x.Title, news.title?.Trim())
            .Set(x => x.Content, news.content?.Trim())
            .Set(x => x.Category, news.category)
            .Set(x => x.ThumbnailUrl, NormalizeThumbnailUrl(news.thumbnailUrl))
            .Update();
    }

    public async Task DeleteNews(long newsId) {
        await _client
            .From<ServerNews>()
            .Where(x => x.Id == newsId)
            .Delete();
    }

    static News ToLocalNews(ServerNews news) {
        return new News {
            id = news.Id,
            title = news.Title,
            content = news.Content,
            category = news.Category,
            thumbnailUrl = news.ThumbnailUrl,
            createdAt = news.CreatedAt,
        };
    }

    static NewsMetadata ToLocalNewsMetadata(ServerNewsMetadata news) {
        return new NewsMetadata {
            id = news.Id,
            title = news.Title,
            category = news.Category,
            thumbnailUrl = news.ThumbnailUrl,
            createdAt = news.CreatedAt,
        };
    }

    static string NormalizeThumbnailUrl(string thumbnailUrl) {
        return string.IsNullOrWhiteSpace(thumbnailUrl) ? null : thumbnailUrl.Trim();
    }
}
