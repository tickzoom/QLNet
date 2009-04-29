using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using QSilver.Infrastructure.Interfaces;
using QSilver.Modules.News.Article;
using QSilver.Modules.News.Controllers;
using QSilver.Modules.News.Services;

namespace QSilver.Modules.News
{
    public class NewsModule : IModule
    {
        private readonly IUnityContainer container;

        public NewsModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            INewsController controller = this.container.Resolve<INewsController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<INewsController, NewsController>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<IArticleView, ArticleView>();
            this.container.RegisterType<IArticlePresentationModel, ArticlePresentationModel>();
            //this.container.RegisterType<INewsFeedService, NewsFeedService>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<INewsFeedService, NewsFeedServiceEDM>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<INewsReaderView, NewsReader>();
            this.container.RegisterType<INewsReaderPresenter, NewsReaderPresenter>();
        }
    }
}
