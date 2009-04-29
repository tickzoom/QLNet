using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using QSilver.Infrastructure;
using QSilver.Infrastructure.Models;
using QSilver.Modules.News.Article;

namespace QSilver.Modules.News.Controllers
{
    public class NewsController : INewsController
    {
        private readonly IRegionManager regionManager;
        private readonly IArticlePresentationModel articlePresentationModel;
        private readonly IEventAggregator eventAggregator;
        private readonly INewsReaderPresenter readerPresenter;
        private readonly IRegion shellRegion;

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "newsReader")]
        public NewsController(IRegionManager regionManager, IArticlePresentationModel articlePresentationModel, IEventAggregator eventAggregator, INewsReaderPresenter newsReaderPresenter)
        {
            this.regionManager = regionManager;
            this.articlePresentationModel = articlePresentationModel;
            this.eventAggregator = eventAggregator;
            this.articlePresentationModel.Controller = this;

            this.readerPresenter = newsReaderPresenter;

            this.shellRegion = this.regionManager.Regions[RegionNames.SecondaryRegion];
            this.shellRegion.Add(this.readerPresenter.View);
        }

        public void Run()
        {
            this.regionManager.Regions[RegionNames.ResearchRegion].Add(articlePresentationModel.View);
            eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(ShowNews, ThreadOption.UIThread);
            eventAggregator.GetEvent<ModuleLoadedEvent>().Subscribe(LoadNews, ThreadOption.UIThread);
        }

        public void LoadNews(string moduleName)
        {
            this.articlePresentationModel.LoadNews(moduleName);
        }

        public void ShowNews(string companySymbol)
        {
            this.articlePresentationModel.SetTickerSymbol(companySymbol);
        }

        public void CurrentNewsArticleChanged(NewsArticle article)
        {
            this.readerPresenter.SetNewsArticle(article);
        }

        public void ShowNewsReader()
        {
            this.shellRegion.Activate(this.readerPresenter.View);
        }
    }
}
