using QSilver.Infrastructure.Models;

namespace QSilver.Modules.News.Article
{
    public interface INewsReaderPresenter
    {
        INewsReaderView View { get; }

        void SetNewsArticle(NewsArticle article);
    }
}
