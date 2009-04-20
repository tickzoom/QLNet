//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using QSilver.Resources;

namespace QSilver.Controls
{
    /// <summary>
    /// Custom Tab control with animations.
    /// </summary>
    /// <remarks>
    /// This customization of the TabControl was required to create the animations for the transition 
    /// between the tab items.
    /// </remarks>
    public class AnimatedTabControl : TabControl
    {
        private TabItem previousSelectedTabItem;
        private FrameworkElement previousSelectedTabItemContent;

        public AnimatedTabControl()
        {
            DefaultStyleKey = typeof(AnimatedTabControl);
        }

        private FrameworkElement CurrentView { get; set; }

        private ContentControl BufferView { get; set; }

        private Storyboard StartingTransition { get; set; }

        private Storyboard EndingTransition { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.CurrentView = (FrameworkElement) this.GetTemplateChild("ContentTop");
            this.BufferView = (ContentControl) this.GetTemplateChild("BufferView");

            FrameworkElement containerGrid = (FrameworkElement) this.GetTemplateChild("LayoutRoot");
            this.StartingTransition = (Storyboard) containerGrid.FindName("StartingTransition");
            this.EndingTransition = (Storyboard) containerGrid.FindName("EndingTransition");
            this.StartingTransition.Completed += this.StartingTransition_Completed;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                this.RestoreBufferedTabItemContent();

                // Put the "old" view in a buffer so we can still show it to perform the starting animation with it
                this.previousSelectedTabItem = (TabItem)args.RemovedItems[0];
                this.previousSelectedTabItemContent = (FrameworkElement)this.previousSelectedTabItem.Content;
                this.previousSelectedTabItem.Content = null;
                this.CurrentView.Visibility = Visibility.Collapsed;
                this.BufferView.Content = this.previousSelectedTabItemContent;

                this.StartingTransition.Begin();
            }
        }

        private void RestoreBufferedTabItemContent()
        {
            if (this.previousSelectedTabItemContent == null || this.previousSelectedTabItem == null)
            {
                return;
            }

            this.BufferView.Content = null;
            this.previousSelectedTabItem.Content = this.previousSelectedTabItemContent;
            this.previousSelectedTabItem = null;
            this.previousSelectedTabItemContent = null;
        }

        private void StartingTransition_Completed(object sender, EventArgs e)
        {
            this.RestoreBufferedTabItemContent();

            this.CurrentView.Visibility = Visibility.Visible;

            // fire transition
            this.EndingTransition.Begin();
        }
    }
}