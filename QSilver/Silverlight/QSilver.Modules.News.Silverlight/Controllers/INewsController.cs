using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSilver.Infrastructure.Models;

namespace QSilver.Modules.News.Controllers
{
    public interface INewsController
    {
        //It may be reasonable to have a Run method instead of relying on the constructor to start it up
        void CurrentNewsArticleChanged(NewsArticle article);
        void ShowNewsReader();
        void Run();
    }
}
