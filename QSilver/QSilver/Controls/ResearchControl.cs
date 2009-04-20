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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace QSilver.Controls
{
    /// <summary>
    /// Custom ItemsControl with Header.
    /// </summary>
    public class ResearchControl : ItemsControl
    {
        public static readonly DependencyProperty HeadersProperty =
            DependencyProperty.Register("Headers", typeof(ObservableCollection<object>), typeof(ResearchControl), null);

        public ResearchControl()
        {
            this.Headers = new ObservableCollection<object>();
        }

        public ObservableCollection<object> Headers
        {
            get { return (ObservableCollection<object>)GetValue(HeadersProperty); }
            private set { SetValue(HeadersProperty, value); }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                object newItem = e.NewItems[0];
                DependencyObject header = GetHeader(newItem as FrameworkElement);
                this.Headers.Insert(e.NewStartingIndex, header);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.Headers.RemoveAt(e.OldStartingIndex);
            }
        }

        private static DependencyObject GetHeader(FrameworkElement view)
        {
            if (view != null)
            {
                DataTemplate template = view.Resources["HeaderIcon"] as DataTemplate;
                if (template != null)
                {
                    return template.LoadContent();
                }
            }

            return null;
        }
    }
}
