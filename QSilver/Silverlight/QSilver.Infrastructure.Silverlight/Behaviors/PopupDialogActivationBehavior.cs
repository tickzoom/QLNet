namespace QSilver.Infrastructure.Behaviors
{
    /// <summary>
    /// Specifies the <see cref="DialogActivationBehavior"/> class for using the behavior on Silverlight.
    /// </summary>
    public class PopupDialogActivationBehavior : DialogActivationBehavior
    {
        /// <summary>
        /// Creates a wrapper for the Silverlight <see cref="System.Windows.Controls.Primitives.Popup"/>.
        /// </summary>
        /// <returns>Instance of the <see cref="System.Windows.Controls.Primitives.Popup"/> wrapper.</returns>
        protected override IWindow CreateWindow()
        {
            return new PopupWrapper();
        }
    }
}
