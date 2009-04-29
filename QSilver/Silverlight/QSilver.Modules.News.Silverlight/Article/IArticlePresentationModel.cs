using QSilver.Modules.News.Controllers;

namespace QSilver.Modules.News.Article
{
    public interface IArticlePresentationModel
    {
        void LoadNews(string moduleName);
        void SetTickerSymbol(string companySymbol);
        IArticleView View { get; }
        INewsController Controller { get; set; }
    }
}
