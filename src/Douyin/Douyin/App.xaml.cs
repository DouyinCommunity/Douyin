﻿using Avalonia;
using Avalonia.Markup.Xaml;

namespace Douyin
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
