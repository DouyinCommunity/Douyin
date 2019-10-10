using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Douyin.Pages
{
    public class ViewboxPage : UserControl
    {
        public ViewboxPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
