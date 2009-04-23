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
using System.ComponentModel;
using System.Collections.Generic;
using QSilver.Infrastructure.Models;
using QSilver.Modules.News.Controllers;
using QSilver.Infrastructure.Interfaces;

namespace QSilver.Modules.News.Article
{
    public class ArticlePresentationModel : IArticlePresentationModel, INotifyPropertyChanged
    {
        private IList<NewsArticle> articles;
        private NewsArticle selectedArticle;
        private readonly INewsFeedService newsFeedService;
        public event PropertyChangedEventHandler PropertyChanged;

        public ArticlePresentationModel(IArticleView view, INewsFeedService newsFeedService)
        {
            View = view;
            View.Model = this;
            this.newsFeedService = newsFeedService;
            View.ShowNewsReader += View_ShowNewsReader;
        }

        public IArticleView View { get; private set; }

        public INewsController Controller { get; set; }

        public void SetTickerSymbol(string companySymbol)
        {
            this.Articles = newsFeedService.GetNews(companySymbol);
        }


        public NewsArticle SelectedArticle
        {
            get { return this.selectedArticle; }
            set
            {
                if (this.selectedArticle != value)
                {
                    this.selectedArticle = value;
                    OnPropertyChanged("SelectedArticle");
                    this.SelectedArticleChanged();
                }
            }
        }

        public IList<NewsArticle> Articles
        {
            get { return this.articles; }
            private set
            {
                if (this.articles != value)
                {
                    this.articles = value;
                    OnPropertyChanged("Articles");
                }
            }
        }

        private void View_ShowNewsReader(object sender, EventArgs e)
        {
            this.Controller.ShowNewsReader();
        }

        private void SelectedArticleChanged()
        {
            this.Controller.CurrentNewsArticleChanged(this.SelectedArticle);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
