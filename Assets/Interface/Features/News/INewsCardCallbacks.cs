using System.Threading.Tasks;

public interface INewsCardCallbacks : ICallbacks {
    Task OnOpenNews(long newsId);
    Task OnEditNews(long newsId);
    Task OnDeleteNews(long newsId);
}
