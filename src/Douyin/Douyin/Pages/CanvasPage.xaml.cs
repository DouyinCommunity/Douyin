using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Douyin.Pages
{
    public class CanvasPage : UserControl
    {
        public CanvasPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
