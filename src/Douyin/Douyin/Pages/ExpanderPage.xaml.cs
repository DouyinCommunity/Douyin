using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Douyin.Pages
{
    public class ExpanderPage : UserControl
    {
        public ExpanderPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
