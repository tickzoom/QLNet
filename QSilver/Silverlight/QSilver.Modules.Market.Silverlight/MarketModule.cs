using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Regions;
using QSilver.Infrastructure;
using QSilver.Modules.Market.Curves;

namespace QSilver.Modules.Market
{
    public class MarketModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public MarketModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            RegisterViewsAndServices();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainRegion, () => this.container.Resolve<ICurvesPresentationModel>().View);
        }

        protected void RegisterViewsAndServices()
        {
            container.RegisterType<ICurvesView, CurvesView>();
            container.RegisterType<ICurvesPresentationModel, CurvesPresentationModel>();

            //container.RegisterType<IMarketHistoryService, MarketHistoryService>(new ContainerControlledLifetimeManager());
            //container.RegisterType<IMarketFeedService, MarketFeedService>(new ContainerControlledLifetimeManager());
            //container.RegisterType<ITrendLineView, TrendLineView>();
            //container.RegisterType<ITrendLinePresentationModel, TrendLinePresentationModel>();
        }

        #endregion

    }
}
