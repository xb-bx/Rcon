using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rcon.Views
{
    public class RemoteConsole : UserControl
    {
        public RemoteConsole()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}