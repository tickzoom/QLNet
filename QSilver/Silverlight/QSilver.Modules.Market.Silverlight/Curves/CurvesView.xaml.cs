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
using QSilver.Modules.Market.Curves;

namespace QSilver.Modules.Market.Curves
{
    public partial class CurvesView : UserControl , ICurvesView
    {
        public CurvesView()
        {
            InitializeComponent();
        }

        public ICurvesPresentationModel Model
        {
            get
            {
                return this.DataContext as ICurvesPresentationModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
