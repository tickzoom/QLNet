
namespace QSilver
{
    public class ShellPresenter
    {
        public ShellPresenter(IShellView view)
        {
            View = view;

        }

        public IShellView View { get; private set; }
    }
}
