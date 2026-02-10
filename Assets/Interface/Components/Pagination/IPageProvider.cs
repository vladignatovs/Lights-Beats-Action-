using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Generic interface to be used by the classes which provide the pagination logic with data and implement
/// a generic LoadPage
/// </summary>
/// <typeparam name="T">Type of data to be provided to the pagination</typeparam>
public interface IPageProvider<T> {
    Task<(List<T> items, int totalCount)> LoadPage(int offset, int pageSize, List<IFilter> filters = null);
}