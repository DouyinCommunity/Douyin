﻿namespace Douyin.FFME.Windows.ViewModels
{
    using Common;
    using Foundation;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents a VM for the Controller Control.
    /// </summary>
    /// <seealso cref="AttachedViewModel" />
    public sealed class ControllerViewModel : AttachedViewModel
    {
        private const string VideoEqContrast = "eq=contrast=";
        private const string VideoEqBrightness = ":brightness=";
        private const string VideoEqSaturation = ":saturation=";

        private Visibility m_IsMediaOpenVisibility = Visibility.Visible;
        private bool m_IsAudioControlEnabled = true;
        private bool m_IsSpeedRatioEnabled = true;
        private Visibility m_ClosedCaptionsVisibility = Visibility.Visible;
        private Visibility m_AudioControlVisibility = Visibility.Visible;
        private Visibility m_PauseButtonVisibility = Visibility.Visible;
        private Visibility m_PlayButtonVisibility = Visibility.Visible;
        private Visibility m_StopButtonVisibility = Visibility.Visible;
        private Visibility m_CloseButtonVisibility = Visibility.Visible;
        private Visibility m_OpenButtonVisibility = Visibility.Visible;
        private Visibility m_SeekBarVisibility = Visibility.Visible;
        private Visibility m_BufferingProgressVisibility = Visibility.Visible;
        private Visibility m_DownloadProgressVisibility = Visibility.Visible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerViewModel"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        internal ControllerViewModel(RootViewModel root)
            : base(root)
        {
            // placeholder
        }

        /// <summary>
        /// Gets or sets the video contrast.
        /// </summary>
        public double VideoContrast
        {
            get => ParseVideoEqualizerFilter().Contrast;
            set => ApplyVideoEqualizerFilter(value, null, null);
        }

        /// <summary>
        /// Gets or sets the video brightness.
        /// </summary>
        public double VideoBrightness
        {
            get => ParseVideoEqualizerFilter().Brightness;
            set => ApplyVideoEqualizerFilter(null, value, null);
        }

        /// <summary>
        /// Gets or sets the video saturation.
        /// </summary>
        public double VideoSaturation
        {
            get => ParseVideoEqualizerFilter().Saturation;
            set => ApplyVideoEqualizerFilter(null, null, value);
        }

        /// <summary>
        /// Gets or sets the is media open visibility.
        /// </summary>
        public Visibility IsMediaOpenVisibility
        {
            get => m_IsMediaOpenVisibility;
            set => SetProperty(ref m_IsMediaOpenVisibility, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is audio control enabled.
        /// </summary>
        public bool IsAudioControlEnabled
        {
            get => m_IsAudioControlEnabled;
            set => SetProperty(ref m_IsAudioControlEnabled, value);
        }

        /// <summary>
        /// Gets or sets the CC channel button control visibility.
        /// </summary>
        public Visibility ClosedCaptionsVisibility
        {
            get => m_ClosedCaptionsVisibility;
            set => SetProperty(ref m_ClosedCaptionsVisibility, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is speed ratio enabled.
        /// </summary>
        public bool IsSpeedRatioEnabled
        {
            get => m_IsSpeedRatioEnabled;
            set => SetProperty(ref m_IsSpeedRatioEnabled, value);
        }

        /// <summary>
        /// Gets or sets the audio control visibility.
        /// </summary>
        public Visibility AudioControlVisibility
        {
            get => m_AudioControlVisibility;
            set => SetProperty(ref m_AudioControlVisibility, value);
        }

        /// <summary>
        /// Gets or sets the pause button visibility.
        /// </summary>
        public Visibility PauseButtonVisibility
        {
            get => m_PauseButtonVisibility;
            set => SetProperty(ref m_PauseButtonVisibility, value);
        }

        /// <summary>
        /// Gets or sets the play button visibility.
        /// </summary>
        public Visibility PlayButtonVisibility
        {
            get => m_PlayButtonVisibility;
            set => SetProperty(ref m_PlayButtonVisibility, value);
        }

        /// <summary>
        /// Gets or sets the stop button visibility.
        /// </summary>
        public Visibility StopButtonVisibility
        {
            get => m_StopButtonVisibility;
            set => SetProperty(ref m_StopButtonVisibility, value);
        }

        /// <summary>
        /// Gets or sets the close button visibility.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get => m_CloseButtonVisibility;
            set => SetProperty(ref m_CloseButtonVisibility, value);
        }

        /// <summary>
        /// Gets or sets the open button visibility.
        /// </summary>
        public Visibility OpenButtonVisibility
        {
            get => m_OpenButtonVisibility;
            set => SetProperty(ref m_OpenButtonVisibility, value);
        }

        /// <summary>
        /// Gets or sets the seek bar visibility.
        /// </summary>
        public Visibility SeekBarVisibility
        {
            get => m_SeekBarVisibility;
            set => SetProperty(ref m_SeekBarVisibility, value);
        }

        /// <summary>
        /// Gets or sets the buffering progress visibility.
        /// </summary>
        public Visibility BufferingProgressVisibility
        {
            get => m_BufferingProgressVisibility;
            set => SetProperty(ref m_BufferingProgressVisibility, value);
        }

        /// <summary>
        /// Gets or sets the download progress visibility.
        /// </summary>
        public Visibility DownloadProgressVisibility
        {
            get => m_DownloadProgressVisibility;
            set => SetProperty(ref m_DownloadProgressVisibility, value);
        }

        /// <summary>
        /// Gets or sets the media element zoom.
        /// </summary>
        public double MediaElementZoom
        {
            get
            {
                var m = App.ViewModel.MediaElement;
                if (m == null) return 1d;

                var transform = m.RenderTransform as ScaleTransform;
                return transform?.ScaleX ?? 1d;
            }
            set
            {
                var m = App.ViewModel.MediaElement;
                if (m == null) return;

                var transform = m.RenderTransform as ScaleTransform;
                if (transform == null)
                {
                    transform = new ScaleTransform(1, 1);
                    m.RenderTransformOrigin = new Point(0.5, 0.5);
                    m.RenderTransform = transform;
                }

                transform.ScaleX = value;
                transform.ScaleY = value;

                if (transform.ScaleX < 0.1d || transform.ScaleY < 0.1)
                {
                    transform.ScaleX = 0.1d;
                    transform.ScaleY = 0.1d;
                }
                else if (transform.ScaleX > 5d || transform.ScaleY > 5)
                {
                    transform.ScaleX = 5;
                    transform.ScaleY = 5;
                }

                NotifyPropertyChanged();
            }
        }

        /// <inheritdoc />
        internal override void OnApplicationLoaded()
        {
            base.OnApplicationLoaded();
            var m = App.ViewModel.MediaElement;

            m.WhenChanged(() => IsMediaOpenVisibility = m.IsOpen ? Visibility.Visible : Visibility.Hidden,
                nameof(m.IsOpen));

            m.WhenChanged(() => ClosedCaptionsVisibility = m.HasClosedCaptions ? Visibility.Visible : Visibility.Hidden,
                nameof(m.HasClosedCaptions));

            m.WhenChanged(() =>
            {
                AudioControlVisibility = m.HasAudio ? Visibility.Visible : Visibility.Hidden;
                IsAudioControlEnabled = m.HasAudio;
            }, nameof(m.HasAudio));

            m.WhenChanged(() => PauseButtonVisibility = m.CanPause && m.IsPlaying ? Visibility.Visible : Visibility.Collapsed,
                nameof(m.CanPause),
                nameof(m.IsPlaying));

            m.WhenChanged(() =>
            {
                PlayButtonVisibility =
                    m.IsOpen && m.IsPlaying == false && m.HasMediaEnded == false && m.IsSeeking == false && m.IsChanging == false ?
                    Visibility.Visible : Visibility.Collapsed;
            },
            nameof(m.IsOpen),
            nameof(m.IsPlaying),
            nameof(m.HasMediaEnded),
            nameof(m.IsSeeking),
            nameof(m.IsChanging));

            m.WhenChanged(() =>
            {
                StopButtonVisibility =
                    m.IsOpen && m.IsChanging == false && m.IsSeeking == false && (m.HasMediaEnded || (m.IsSeekable && m.MediaState != MediaPlaybackState.Stop)) ?
                    Visibility.Visible : Visibility.Hidden;
            },
            nameof(m.IsOpen),
            nameof(m.HasMediaEnded),
            nameof(m.IsSeekable),
            nameof(m.MediaState),
            nameof(m.IsChanging),
            nameof(m.IsSeeking));

            m.WhenChanged(() => CloseButtonVisibility = (m.IsOpen || m.IsOpening) ? Visibility.Visible : Visibility.Hidden,
                nameof(m.IsOpen),
                nameof(m.IsOpening));

            m.WhenChanged(() => SeekBarVisibility = m.IsSeekable ? Visibility.Visible : Visibility.Hidden, nameof(m.IsSeekable));

            m.WhenChanged(() =>
            {
                BufferingProgressVisibility = m.IsOpening || (m.IsBuffering && m.BufferingProgress < 0.95)
                    ? Visibility.Visible
                    : Visibility.Hidden;
            },
            nameof(m.IsOpening),
            nameof(m.IsBuffering),
            nameof(m.BufferingProgress),
            nameof(m.Position));

            m.WhenChanged(() =>
            {
                DownloadProgressVisibility = m.IsOpen && m.HasMediaEnded == false &&
                    ((m.DownloadProgress > 0d && m.DownloadProgress < 0.95) || m.IsLiveStream)
                        ? Visibility.Visible
                        : Visibility.Hidden;
            },
            nameof(m.IsOpen),
            nameof(m.HasMediaEnded),
            nameof(m.DownloadProgress),
            nameof(m.IsLiveStream));

            m.WhenChanged(() => OpenButtonVisibility = m.IsOpening == false ? Visibility.Visible : Visibility.Hidden, nameof(m.IsOpening));

            m.WhenChanged(() => IsSpeedRatioEnabled = m.IsOpening == false, nameof(m.IsOpen), nameof(m.IsSeekable));
        }

        private EqualizerFilterValues ParseVideoEqualizerFilter()
        {
            var result = new EqualizerFilterValues() { Contrast = 1d, Brightness = 0d, Saturation = 1d };

            if (Root.MediaElement == null || Root.MediaElement.HasVideo == false) return result;

            var currentFilter = Root.CurrentMediaOptions?.VideoFilter;
            if (string.IsNullOrWhiteSpace(currentFilter)) return result;

            var cIx = currentFilter.LastIndexOf(VideoEqContrast, StringComparison.Ordinal);
            var bIx = currentFilter.LastIndexOf(VideoEqBrightness, StringComparison.Ordinal);
            var sIx = currentFilter.LastIndexOf(VideoEqSaturation, StringComparison.Ordinal);

            if (cIx < 0 || bIx < 0 || sIx < 0) return result;

            var cLiteral = currentFilter.Substring(cIx + VideoEqContrast.Length, 6);
            var bLiteral = currentFilter.Substring(bIx + VideoEqBrightness.Length, 6);
            var sLiteral = currentFilter.Substring(sIx + VideoEqSaturation.Length, 6);

            result.Contrast = double.Parse(cLiteral, CultureInfo.InvariantCulture);
            result.Brightness = double.Parse(bLiteral, CultureInfo.InvariantCulture);
            result.Saturation = double.Parse(sLiteral, CultureInfo.InvariantCulture);

            return result;
        }

        private void ApplyVideoEqualizerFilter(double? contrast, double? brightness, double? saturation)
        {
            if (Root.MediaElement == null || Root.MediaElement.HasVideo == false || Root.CurrentMediaOptions == null)
                return;

            try
            {
                var currentValues = ParseVideoEqualizerFilter();

                contrast = contrast == null ? currentValues.Contrast : contrast < -2d ? -2d : contrast > 2d ? 2d : contrast;
                brightness = brightness == null ? currentValues.Brightness : brightness < -1d ? -1d : brightness > 1d ? 1d : brightness;
                saturation = saturation == null ? currentValues.Saturation : saturation < 0d ? 0d : saturation > 3d ? 3d : saturation;

                var targetFilter = $"{VideoEqContrast}{contrast:+0.000;-0.000}{VideoEqBrightness}{brightness:+0.000;-0.000}{VideoEqSaturation}{saturation:+0.000;-0.000}";
                var currentFilter = Root.CurrentMediaOptions?.VideoFilter;

                if (string.IsNullOrWhiteSpace(currentFilter))
                {
                    Root.CurrentMediaOptions.VideoFilter = targetFilter;
                    return;
                }

                var cIx = currentFilter.LastIndexOf(VideoEqContrast, StringComparison.Ordinal);
                Root.CurrentMediaOptions.VideoFilter = cIx < 0
                    ? $"{currentFilter},{targetFilter}"
                    : currentFilter.Substring(0, cIx) + targetFilter;
            }
            finally
            {
                NotifyPropertyChanged(nameof(VideoContrast));
                NotifyPropertyChanged(nameof(VideoBrightness));
                NotifyPropertyChanged(nameof(VideoSaturation));

                // Notify a change in Video Equalizer
                Root.NotificationMessage = $"Contrast:   {contrast:+0.000;-0.000}\r\nBrightness: {brightness:+0.000;-0.000}\r\nSaturation: {saturation:+0.000;-0.000}";
            }
        }

        private struct EqualizerFilterValues
        {
            public double Contrast;
            public double Brightness;
            public double Saturation;
        }
    }
}
