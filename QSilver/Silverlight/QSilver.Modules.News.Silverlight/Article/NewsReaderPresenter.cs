using QSilver.Infrastructure.Models;

namespace QSilver.Modules.News.Article
{
    public class NewsReaderPresenter : INewsReaderPresenter
    {
        private readonly INewsReaderView view;

        public NewsReaderPresenter(INewsReaderView view)
        {
            this.view = view;
        }

        public INewsReaderView View
        {
            get { return this.view; }
        }

        public void SetNewsArticle(NewsArticle article)
        {
            this.view.Model = article;
        }
    }
}
