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

namespace QSilver.Modules.Market.Curves
{
    public class CurvesPresentationModel : ICurvesPresentationModel
    {

        public CurvesPresentationModel(ICurvesView view)
        {
            View = view;
            View.Model = this;
        }


        public ICurvesView View { get; set; }

        public string HeaderInfo
        {
            get { return "CURVES"; }
        }

    }
}
