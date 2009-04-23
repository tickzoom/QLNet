using System;

namespace QSilver.Modules.News.Article
{
    public interface IArticleView
    {
        event EventHandler<EventArgs> ShowNewsReader;

        ArticlePresentationModel Model { get; set; }
    }
}
