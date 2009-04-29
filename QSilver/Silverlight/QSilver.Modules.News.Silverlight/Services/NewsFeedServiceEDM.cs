using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using QSilver.Infrastructure.Interfaces;
using QSilver.Infrastructure.Models;
using QSilver.Modules.News.Properties;
using System.Linq;

namespace QSilver.Modules.News.Services
{
    using QSilverSvc;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Globalization;
    using QSilver.Infrastructure;
    using Microsoft.Practices.Composite.Events;

    public class NewsFeedServiceEDM : INewsFeedService
    {
        Dictionary<string, List<NewsArticle>> newsData = new Dictionary<string, List<NewsArticle>>();

        QSilverEntities svc = new QSilverEntities(new Uri("http://localhost:54303/Services/QSilver.svc/", UriKind.Absolute));

        private readonly IEventAggregator eventAggregator;

        public NewsFeedServiceEDM(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private void FindNews_Completed(IAsyncResult result)
        {
            DataServiceQuery<news> query = (DataServiceQuery<news>)result.AsyncState;

            try
            {
                newsData = query.EndExecute(result)
                                .GroupBy(x => x.Module,
                                x => new NewsArticle
                                {
                                    PublishedDate = (DateTime)x.PublishedDate,
                                    Title = x.Title,
                                    Body = x.Body,
                                    IconUri = x.IconUri != null ? x.IconUri : null
                                })
                                .ToDictionary(group => group.Key, group => group.ToList());

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to retrieve data : " + ex.ToString());
            }

            List<NewsArticle> articles = new List<NewsArticle>();
            foreach (KeyValuePair<string, List<NewsArticle>> kvp in newsData)
            {
                articles = kvp.Value;          
            }

            //articles = (List<NewsArticle>)newsData.Values;

            this.eventAggregator.GetEvent<NewsLoadedEvent>().Publish(articles);

        }

        public void LoadNews(string ModuleName)
        {
            var q = (from n in svc.news
                     where n.Module == ModuleName
                     orderby n.PublishedDate descending
                     select n) as DataServiceQuery<news>;

            q.BeginExecute(new AsyncCallback(FindNews_Completed), q);

        }


        #region INewsFeed Members

        public IList<NewsArticle> GetNews(string tickerSymbol)
        {
            List<NewsArticle> articles = new List<NewsArticle>();
            newsData.TryGetValue(tickerSymbol, out articles);
            return articles;
        }

        public event EventHandler<NewsFeedEventArgs> Updated = delegate { };

        public bool HasNews(string tickerSymbol)
        {
            return newsData.ContainsKey(tickerSymbol);
        }

        #endregion
    }
}
