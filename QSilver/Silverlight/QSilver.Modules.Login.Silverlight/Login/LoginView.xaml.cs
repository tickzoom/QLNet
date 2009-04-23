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

namespace QSilver.Modules.Login.Login
{
    public partial class LoginView : UserControl, ILoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public ILoginPresentationModel Model
        {
            get
            {
                return this.DataContext as ILoginPresentationModel;
            }
            set
            {
                DataContext = value;
            }
        }

    }
}
