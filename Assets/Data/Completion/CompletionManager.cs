using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using static Supabase.Postgrest.Constants;

public class CompletionManager : DataManager {
    public CompletionManager(Client client) : base(client) { }

    public async Task CompleteLevel(Guid serverId, Completion completion) {
        var serverCompletion = new ServerCompletion {
            LevelId = serverId,
            Accuracy = completion.accuracy,
            Percentage = completion.percentage,
            Attempts = completion.attempts
        };

        try {
            await _client
                .From<ServerCompletion>()
                .Insert(serverCompletion);
            return;
        } catch { }

        try {
            await _client
                .From<ServerCompletion>()
                .Where(x => x.LevelId == serverId)
                .Set(x => x.Accuracy, serverCompletion.Accuracy)
                .Set(x => x.Percentage, serverCompletion.Percentage)
                .Set(x => x.Attempts, serverCompletion.Attempts)
                .Update();
        } catch { }
    }

    public async Task<Dictionary<Guid, Completion>> GetCompletionsByLevelIds(List<Guid> levelIds) {
        if (levelIds == null || levelIds.Count == 0) return new Dictionary<Guid, Completion>();

        var levelIdFilters = levelIds
            .Select(id => (object)id)
            .ToList();

        var response = await _client
            .From<ServerCompletion>()
            .Filter("level_id", Operator.In, levelIdFilters)
            .Get();

        var models = response.Models;
        if (models == null || models.Count == 0) return new Dictionary<Guid, Completion>();

        var result = new Dictionary<Guid, Completion>(models.Count);

        foreach (var model in models) {
            result[model.LevelId] = new Completion {
                percentage = model.Percentage,
                attempts = model.Attempts,
                accuracy = model.Accuracy
            };
        }

        return result;
    }
}
