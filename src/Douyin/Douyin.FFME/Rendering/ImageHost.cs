﻿namespace Douyin.FFME.Rendering
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Implements an Image control that is hosted on its own independent dispatcher
    /// but maintains composability with the main UI.
    /// </summary>
    internal sealed class ImageHost : ElementHostBase<Image>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageHost"/> class.
        /// </summary>
        public ImageHost()
            : base(true)
        {
            // placeholder
        }

        public ImageHost(bool hasOwnDispatcher)
            : base(hasOwnDispatcher)
        {
            // placeholder
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public ImageSource Source
        {
            get => GetElementProperty<ImageSource>(Image.SourceProperty);
            set => SetElementProperty(Image.SourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the stretch.
        /// </summary>
        public Stretch Stretch
        {
            get => GetElementProperty<Stretch>(Image.StretchProperty);
            set => SetElementProperty(Image.StretchProperty, value);
        }

        /// <summary>
        /// Gets or sets the stretch direction.
        /// </summary>
        public StretchDirection StretchDirection
        {
            get => GetElementProperty<StretchDirection>(Image.StretchDirectionProperty);
            set => SetElementProperty(Image.StretchDirectionProperty, value);
        }

        /// <inheritdoc />
        protected override Image CreateHostedElement()
        {
            var control = new Image();
            control.BeginInit();
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.VerticalAlignment = VerticalAlignment.Stretch;
            control.EndInit();
            return control;
        }
    }
}
