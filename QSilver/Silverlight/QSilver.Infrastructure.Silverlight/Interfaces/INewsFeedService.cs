using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSilver.Infrastructure.Models;

namespace QSilver.Infrastructure.Interfaces
{
    public interface INewsFeedService
    {
        IList<NewsArticle> GetNews(string tickerSymbol);
        bool HasNews(string tickerSymbol);
        event EventHandler<NewsFeedEventArgs> Updated;
        void LoadNews(string moduleName);


    }
}
