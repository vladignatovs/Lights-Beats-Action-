using System;
using System.Linq.Expressions;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;

public class WhereFilter<T> : IDataFilter<T> where T : BaseModel, new() {
    readonly Expression<Func<T, bool>> _predicate;

    public WhereFilter(Expression<Func<T, bool>> predicate) {
        _predicate = predicate;
    }

    public IPostgrestTable<T> Apply(IPostgrestTable<T> query) {
        return query.Where(_predicate);
    }
}