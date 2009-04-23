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
using QSilver.Infrastructure;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Composite.Modularity;
using QSilver.Modules.Login.Login;

namespace QSilver.Modules.Login
{
    public class LoginModule: IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public LoginModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            RegisterViewsAndServices();

            this.regionManager.RegisterViewWithRegion(RegionNames.MainRegion, () => this.container.Resolve<ILoginPresentationModel>().View);
        }

        protected void RegisterViewsAndServices()
        {

            container.RegisterType<ILoginView, LoginView>();
            container.RegisterType<ILoginPresentationModel, LoginPresentationModel>();
        }

        #endregion

    }
}
