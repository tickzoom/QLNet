using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace QSilver
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // Se l'applicazione è in esecuzione all'esterno del debugger, segnalare l'eccezione mediante
            // il meccanismo di eccezioni del browser. In IE verrà visualizzato un messaggio di avviso di colore giallo 
            // un'icona nella barra di stato e in Firefox verrà visualizzato un errore di script.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTA: in questo modo sarà possibile continuare l'esecuzione di un'applicazione dopo la generazione di un'eccezione
                // ma non gestita. 
                // Per le applicazioni di produzione la gestione degli errori deve essere sostituita da un meccanismo che 
                // segnala l'errore al sito Web e interrompe l'applicazione.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
