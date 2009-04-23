using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace QSilver.Infrastructure.Behaviors
{
    /// <summary>
    /// Defines a wrapper for the <see cref="Popup"/> class that implements the <see cref="IWindow"/> interface.
    /// </summary>
    public class PopupWrapper : IWindow
    {
        private readonly Popup popUp;
        private readonly ContentControl container;
        private FrameworkElement owner;

        /// <summary>
        /// Initializes a new instance of <see cref="PopupWrapper"/>.
        /// </summary>
        public PopupWrapper()
        {
            this.container = new ContentControl();

            this.popUp = new Popup();
            this.popUp.Child = this.container;
        }

        /// <summary>
        /// Ocurrs when the <see cref="Popup"/> is closed.
        /// </summary>
        public event EventHandler Closed
        {
            add { this.popUp.Closed += value; }
            remove { this.popUp.Closed -= value; }
        }

        /// <summary>
        /// Gets or Sets the content for the <see cref="Popup"/>.
        /// </summary>
        public object Content
        {
            get
            {
                return this.container.Content;
            }

            set
            {
                if (value != null)
                {
                    this.container.Content = value;
                }
                else
                {
                    this.container.Content = null;
                    this.popUp.Child = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the owner of the <see cref="Popup"/> which is used for resizing.
        /// </summary>
        public object Owner
        {
            get
            {
                return this.owner;
            }

            set
            {
                if (this.owner != null)
                {
                    this.owner.SizeChanged -= this.OwnerSizeChanged;
                }

                this.owner = value as FrameworkElement;
                if (this.owner != null)
                {
                    this.container.Width = this.owner.ActualWidth;
                    this.container.Height = this.owner.ActualHeight;
                    this.owner.SizeChanged += this.OwnerSizeChanged;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="FrameworkElement.Style"/> to apply to the <see cref="Popup"/>.
        /// </summary>
        public Style Style
        {
            get { return this.container.Style; }
            set { this.container.Style = value; }
        }

        /// <summary>
        /// Opens the <see cref="Popup"/>.
        /// </summary>
        public void Show()
        {
            this.popUp.IsOpen = true;
        }

        /// <summary>
        /// Closes the <see cref="Popup"/>.
        /// </summary>
        public void Close()
        {
            this.popUp.IsOpen = false;
        }

        private void OwnerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.container != null)
            {
                this.container.Width = this.owner.ActualWidth;
                this.container.Height = this.owner.ActualHeight;
            }
        }
    }
}
