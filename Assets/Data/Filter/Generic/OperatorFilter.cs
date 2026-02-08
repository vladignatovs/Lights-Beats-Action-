using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;

/// <summary>
/// Filter using Postgrest operators (.Filter method)
/// </summary>
public class OperatorFilter<T> : IDataFilter<T> where T : BaseModel, new() {
    readonly string _column;
    readonly Operator _operator;
    readonly object _value;

    public OperatorFilter(string column, Operator op, object value) {
        _column = column;
        _operator = op;
        _value = value;
    }

    public IPostgrestTable<T> Apply(IPostgrestTable<T> query) {
        return query.Filter(_column, _operator, _value);
    }
}