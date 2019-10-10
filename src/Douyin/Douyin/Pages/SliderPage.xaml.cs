using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Douyin.Pages
{
    public class SliderPage : UserControl
    {
        public SliderPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
