using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;

/// <summary>
/// Generic filter which can be applied to the supabase model queries
/// </summary>
public interface IDataFilter<T> : IFilter where T : BaseModel, new() {
    IPostgrestTable<T> Apply(IPostgrestTable<T> query);
}
