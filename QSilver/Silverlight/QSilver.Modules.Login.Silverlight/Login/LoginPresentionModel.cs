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
using Microsoft.Practices.Composite.Events;
using QSilver.Infrastructure;
using System.ComponentModel;

namespace QSilver.Modules.Login.Login
{
    public class LoginPresentationModel : ILoginPresentationModel, INotifyPropertyChanged
    {
        private string tickerSymbol;

        public LoginPresentationModel(ILoginView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.Model = this;
            eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(this.TickerSymbolChanged);
            TickerSymbolChanged("STOCK0");
        }


        public ILoginView View { get; set; }

        public string HeaderInfo
        {
            get { return "LOGIN"; }
        }

        public void TickerSymbolChanged(string newTickerSymbol)
        {
            this.TickerSymbol = newTickerSymbol;
        }

        public string TickerSymbol
        {
            get
            {
                return this.tickerSymbol;
            }
            set
            {
                if (this.tickerSymbol != value)
                {
                    this.tickerSymbol = value;
                    this.InvokePropertyChanged("TickerSymbol");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
