
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using UnityEngine;
using static Supabase.Postgrest.Constants;

public class CompletionManager : DataManager {
    public CompletionManager(Client client) : base(client) { }

    // what
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
            Debug.Log("[CompletionManager] Inserted completion row.");
            return;
        } catch (Exception e) {
            if (!IsDuplicateKeyError(e)) {
                Debug.LogError("[CompletionManager] Insert failed (not duplicate). " + e.Message);
                return;
            }

            Debug.Log("[CompletionManager] Insert hit duplicate key, trying update.");
        }

        try {
            await _client
                .From<ServerCompletion>()
                .Where(x => x.LevelId == serverId)
                .Set(x => x.Accuracy, serverCompletion.Accuracy)
                .Set(x => x.Percentage, serverCompletion.Percentage)
                .Set(x => x.Attempts, serverCompletion.Attempts)
                .Update();
            Debug.Log("[CompletionManager] Updated completion row.");
        } catch (Exception e) {
            Debug.LogError("[CompletionManager] Update after duplicate insert failed. " + e.Message);
        }
    }

    static bool IsDuplicateKeyError(Exception e) {
        if (e.Message != null && e.Message.Contains("23505")) return true;

        var sqlStateProp = e.GetType().GetProperty("SqlState");
        if (sqlStateProp != null) {
            var sqlState = sqlStateProp.GetValue(e) as string;
            if (sqlState == "23505") return true;
        }

        return false;
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
