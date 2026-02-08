using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Supabase;
using Supabase.Postgrest.Interfaces;
using UnityEngine;
using static Supabase.Postgrest.Constants;

// TODO: show level name in pause menu
// if everythings fine then implement level search, only then would you need to finish
// user account and its search/browse via found level, and then pick up other side-stuff like blocks, friends and bla-bla-bla
public class LevelManager : DataManager {
    public LevelManager(Client client) : base(client) {}

    public async Task<(List<LevelMetadata> items, int totalCount)> LazyLoadLevels(
        int offset, 
        int limit, 
        List<IDataFilter<ServerLevelMetadata>> filters = null
    ) {
        var table = _client.From<ServerLevelMetadata>();
        var query = table.Select("id,creator_id,creator_username,name,audio_path,bpm");

        // Apply filters to the query
        if (filters != null) {
            foreach (var filter in filters) {
                query = filter.Apply(query);
            }
        }

        // Build separate count query with same filters
        IPostgrestTable<ServerLevelMetadata> countQuery = _client.From<ServerLevelMetadata>();
        if (filters != null) {
            foreach (var filter in filters) {
                countQuery = filter.Apply(countQuery);
            }
        }
        
        var getTask = query.Range(offset, offset + limit - 1).Get();
        var countTask = countQuery.Count(CountType.Exact); // TODO: might rethink this approach for scalability
        
        // run both pagination and count queries in parallel
        await Task.WhenAll(getTask, countTask);

        var response = await getTask;
        int totalCount = await countTask;

        var items = (response.Models ?? new())
            .Select(ToLocalLevelMetadata)
            .ToList();

        return (items, totalCount);
    }

    public async Task<Level> LoadLevel(Guid serverId) {
        var getResponse = await _client
            .From<ServerLevel>()
            .Where(x => x.Id == serverId)
            .Get();
        var model = getResponse.Model;
        return new() {
            serverId = model.Id,
            name = model.Name,
            bpm = model.Bpm,
            audioPath = model.AudioPath,
            actions = (model.Actions ?? new List<ServerAction>())
                .Select(ToLocalAction)
                .ToList(),
        };
    }

    public async Task DeleteLevel(Guid serverId) {
        await _client
            .From<ServerLevel>()
            .Where(x => x.Id == serverId)
            .Delete();
    }

    /// <summary>
    /// Creates a ServerLevel instance out of a local Level and puts it in a database
    /// </summary>
    /// <param name="level"></param>
    public async Task<Level> PublishLevel(Level level) {
        var serverLevel = new ServerLevel
        {
            Name = level.name,
            Bpm = level.bpm,
            AudioPath = level.audioPath,
            Actions = level.actions.ConvertAll(a => new ServerAction
            {
                Beat = a.Beat,
                Times = a.Times,
                Delay = a.Delay,
                GObject = a.GObject,
                PositionX = a.PositionX,
                PositionY = a.PositionY,
                Rotation = a.Rotation,
                ScaleX = a.ScaleX,
                ScaleY = a.ScaleY,
                AnimationDuration = a.AnimationDuration,
                LifeTime = a.LifeTime,
                Groups = new JArray(a.Groups)
            })
        };

        // if there is a server id, try to update the connected server level, if cant, fallback and create a new level
        if(level.serverId.HasValue) {
            try {
                // TODO: separate metadata update and actions list update
                var updateResponse = await _client
                    .From<ServerLevel>()
                    .Where(x => x.Id == level.serverId.Value)
                    .Set(x => x.Name, serverLevel.Name)
                    .Set(x => x.AudioPath, serverLevel.AudioPath)
                    .Set(x => x.Bpm, serverLevel.Bpm)
                    .Set(x => x.Actions, serverLevel.Actions)
                    .Update();

                var updated = updateResponse?.Model;
                level.serverId = updated.Id;
                return level;
            } catch (Exception e) {
                Debug.Log("Couldn't update, fallback to regular insert. " + e);
            }
        }

        var insertResponse = await _client
            .From<ServerLevel>()
            .Insert(serverLevel);

        var inserted = insertResponse?.Model;

        level.serverId = inserted.Id;

        return level;
    }

    private static Action ToLocalAction(ServerAction sa)
    {
        return new Action
        {
            Beat = sa.Beat,
            Times = sa.Times,
            Delay = sa.Delay,
            GObject = sa.GObject,
            PositionX = sa.PositionX,
            PositionY = sa.PositionY,
            Rotation = sa.Rotation,
            ScaleX = sa.ScaleX,
            ScaleY = sa.ScaleY,
            AnimationDuration = sa.AnimationDuration,
            LifeTime = sa.LifeTime,
            Groups = sa.Groups != null ? sa.Groups.ToObject<List<int>>() : new List<int>{0}
        };
    }

    private static LevelMetadata ToLocalLevelMetadata(ServerLevelMetadata slm) {
        return new LevelMetadata
        {
            serverId = slm.Id,
            creatorId = slm.CreatorId,
            creatorUsername = slm.CreatorUsername,
            name = slm.Name,
            bpm = slm.Bpm,
            audioPath = slm.AudioPath,
        };
    }
}