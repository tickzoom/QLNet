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
using Microsoft.Practices.Composite.UnityExtensions;


namespace QSilver
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            ShellPresenter presenter = Container.Resolve<ShellPresenter>();
            IShellView view = presenter.View;

            view.ShowView();

            return view as DependencyObject;

        }

        protected override void ConfigureContainer()
        {
            Container.RegisterType<IShellView, Shell>();

            base.ConfigureContainer();
        }

        protected override IModuleCatalog GetModuleCatalog()
        {
            ModuleCatalog catalog = new ModuleCatalog();
            catalog.AddModule(typeof(QSilver.Modules.Market.MarketModule));
            return catalog;
        }

    }
}
