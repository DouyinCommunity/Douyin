namespace Douyin.FFME
{
    using ClosedCaptions;
    using Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class MediaElement
    {
        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        public MediaPlaybackState MediaState => MediaCore?.State.MediaState ?? MediaPlaybackState.Close;

        /// <summary>
        /// Gets the Media's natural duration
        /// Only valid after the MediaOpened event has fired.
        /// Returns null for undefined or negative duration.
        /// </summary>
        public TimeSpan? NaturalDuration
        {
            get
            {
                var duration = MediaCore?.State.NaturalDuration;

                return !duration.HasValue || duration.Value < TimeSpan.Zero
                  ? default
                  : duration.Value;
            }
        }

        /// <summary>
        /// Gets the media playback start time. Useful for slider minimum values.
        /// </summary>
        public TimeSpan? PlaybackStartTime => MediaCore?.State.PlaybackStartTime;

        /// <summary>
        /// Gets the media playback end time. Useful for slider maximum values.
        /// </summary>
        public TimeSpan? PlaybackEndTime => MediaCore?.State.PlaybackEndTime;

        /// <summary>
        /// Gets the remaining playback duration. Returns null for indeterminate values.
        /// </summary>
        public TimeSpan? RemainingDuration
        {
            get
            {
                if (PlaybackEndTime.HasValue == false || IsSeekable == false) return default;
                return PlaybackEndTime.Value.Ticks <= Position.Ticks
                    ? TimeSpan.Zero
                    : TimeSpan.FromTicks(PlaybackEndTime.Value.Ticks - Position.Ticks);
            }
        }

        /// <summary>
        /// Gets the actual playback position. Differently from the <see cref="Position"/> property,
        /// this is not a dependency property. Reading this property will return the media engine's
        /// internal position.
        /// </summary>
        public TimeSpan? ActualPosition => MediaCore?.PlaybackPosition ?? default;

        /// <summary>
        /// Gets the discrete time position of the start of the current
        /// frame of the main component.
        /// </summary>
        public TimeSpan FramePosition => MediaCore?.State.FramePosition ?? default;

        /// <summary>
        /// Gets the stream's total bit rate as reported by the container.
        /// Returns 0 if unavailable.
        /// </summary>
        public long BitRate => MediaCore?.State.BitRate ?? default;

        /// <summary>
        /// Gets the instantaneous, compressed bit rate of the decoders for the currently active component streams.
        /// This is provided in bits per second.
        /// </summary>
        public long DecodingBitRate => MediaCore?.State.DecodingBitRate ?? default;

        /// <summary>
        /// Provides key-value pairs of the metadata contained in the media.
        /// Returns null when media has not been loaded.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata => MediaCore?.State.Metadata;

        /// <summary>
        /// Provides stream, chapter and program info of the underlying media.
        /// Returns null when no media is loaded.
        /// </summary>
        public MediaInfo MediaInfo => MediaCore?.MediaInfo;

        /// <summary>
        /// Gets the index of the video stream.
        /// </summary>
        public int VideoStreamIndex => MediaCore?.State.VideoStreamIndex ?? -1;

        /// <summary>
        /// Gets the index of the audio stream.
        /// </summary>
        public int AudioStreamIndex => MediaCore?.State.AudioStreamIndex ?? -1;

        /// <summary>
        /// Gets the index of the subtitle stream.
        /// </summary>
        public int SubtitleStreamIndex => MediaCore?.State.SubtitleStreamIndex ?? -1;

        /// <summary>
        /// Gets the media format. Returns null when media has not been loaded.
        /// </summary>
        public string MediaFormat => MediaCore?.State.MediaFormat;

        /// <summary>
        /// Gets the size in bytes of the current stream being read.
        /// For multi-file streams, get the size of the current file only.
        /// </summary>
        public long MediaStreamSize => MediaCore?.State.MediaStreamSize ?? 0;

        /// <summary>
        /// Gets the duration of a single frame step.
        /// If there is a video component with a frame rate, this property returns the length of a frame.
        /// If there is no video component it simply returns a tenth of a second.
        /// </summary>
        public TimeSpan PositionStep => MediaCore?.State.PositionStep ?? TimeSpan.Zero;

        /// <summary>
        /// Returns whether the given media has audio.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public bool HasAudio => MediaCore?.State.HasAudio ?? default;

        /// <summary>
        /// Returns whether the given media has video. Only valid after the
        /// MediaOpened event has fired.
        /// </summary>
        public bool HasVideo => MediaCore?.State.HasVideo ?? default;

        /// <summary>
        /// Returns whether the given media has subtitles. Only valid after the
        /// MediaOpened event has fired.
        /// </summary>
        public bool HasSubtitles => MediaCore?.State.HasSubtitles ?? false;

        /// <summary>
        /// Gets the video codec.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public string VideoCodec => MediaCore?.State.VideoCodec;

        /// <summary>
        /// Gets the video bit rate.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public long VideoBitRate => MediaCore?.State.VideoBitRate ?? default;

        /// <summary>
        /// Returns the clockwise angle that needs to be applied to the video for it to be displayed
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public double VideoRotation => MediaCore?.State.VideoRotation ?? default;

        /// <summary>
        /// Returns the natural width of the media in the video.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public int NaturalVideoWidth => MediaCore?.State.NaturalVideoWidth ?? default;

        /// <summary>
        /// Returns the natural height of the media in the video.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public int NaturalVideoHeight => MediaCore?.State.NaturalVideoHeight ?? default;

        /// <summary>
        /// Gets the video frame rate.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public double VideoFrameRate => MediaCore?.State.VideoFrameRate ?? default;

        /// <summary>
        /// Gets the name of the video hardware decoder in use.
        /// Enabling hardware acceleration does not guarantee decoding will be performed in hardware.
        /// When hardware decoding of frames is in use this will return the name of the HW accelerator.
        /// Otherwise it will return an empty string.
        /// </summary>
        public string VideoHardwareDecoder => MediaCore?.State.VideoHardwareDecoder;

        /// <summary>
        /// Gets the audio codec.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public string AudioCodec => MediaCore?.State.AudioCodec;

        /// <summary>
        /// Gets the audio bit rate.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public long AudioBitRate => MediaCore?.State.AudioBitRate ?? default;

        /// <summary>
        /// Gets the audio channels count.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public int AudioChannels => MediaCore?.State.AudioChannels ?? default;

        /// <summary>
        /// Gets the audio sample rate.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public int AudioSampleRate => MediaCore?.State.AudioSampleRate ?? default;

        /// <summary>
        /// Gets the audio bits per sample.
        /// Only valid after the MediaOpened event has fired.
        /// </summary>
        public int AudioBitsPerSample => MediaCore?.State.AudioBitsPerSample ?? default;

        /// <summary>
        /// Returns whether the currently loaded media can be paused.
        /// This is only valid after the MediaOpened event has fired.
        /// Note that this property is computed based on whether the stream is detected to be a live stream.
        /// </summary>
        public bool CanPause => MediaCore?.State.CanPause ?? default;

        /// <summary>
        /// Gets a value indicating whether the meadia container has reached the end of the stream.
        /// </summary>
        public bool IsAtEndOfStream => MediaCore?.State.IsAtEndOfStream ?? default;

        /// <summary>
        /// Returns whether the currently loaded media is live or realtime
        /// This is only valid after the MediaOpened event has fired.
        /// </summary>
        public bool IsLiveStream => MediaCore?.State.IsLiveStream ?? default;

        /// <summary>
        /// Returns whether the currently loaded media is a network stream.
        /// This is only valid after the MediaOpened event has fired.
        /// </summary>
        public bool IsNetworkStream => MediaCore?.State.IsNetworkStream ?? default;

        /// <summary>
        /// Gets a value indicating whether the currently loaded media can be seeked.
        /// </summary>
        public bool IsSeekable => MediaCore?.State.IsSeekable ?? default;

        /// <summary>
        /// Gets a value indicating whether the media is playing.
        /// </summary>
        public bool IsPlaying => MediaCore?.State.IsPlaying ?? default;

        /// <summary>
        /// Gets a value indicating whether the media is playing.
        /// </summary>
        public bool IsPaused => MediaCore?.State.IsPaused ?? default;

        /// <summary>
        /// Gets a value indicating whether the media has reached its end.
        /// </summary>
        public bool HasMediaEnded => MediaCore?.State.HasMediaEnded ?? default;

        /// <summary>
        /// Get a value indicating whether the media is buffering.
        /// </summary>
        public bool IsBuffering => MediaCore?.State.IsBuffering ?? default;

        /// <summary>
        /// Gets a value indicating whether the media seeking is in progress.
        /// </summary>
        public bool IsSeeking => MediaCore?.State.IsSeeking ?? default;

        /// <summary>
        /// Returns the current video SMTPE time code if available.
        /// </summary>
        public string VideoSmtpeTimeCode => MediaCore?.State.VideoSmtpeTimeCode;

        /// <summary>
        /// Gets the current video aspect ratio if available.
        /// </summary>
        public string VideoAspectRatio => MediaCore?.State.VideoAspectRatio;

        /// <summary>
        /// Gets a value that indicates the percentage of buffering progress made.
        /// Range is from 0 to 1.
        /// </summary>
        public double BufferingProgress => MediaCore?.State.BufferingProgress ?? default;

        /// <summary>
        /// Gets a value that indicates the percentage of download progress made.
        /// Range is from 0 to 1.
        /// </summary>
        public double DownloadProgress => MediaCore?.State.DownloadProgress ?? default;

        /// <summary>
        /// Gets the amount of bytes in the packet buffer for the active stream components.
        /// </summary>
        public long PacketBufferLength => MediaCore?.State.PacketBufferLength ?? default;

        /// <summary>
        /// Gets the number of packets buffered for all components.
        /// </summary>
        public int PacketBufferCount => MediaCore?.State.PacketBufferCount ?? default;

        /// <summary>
        /// Gets a value indicating whether the media is in the process of opening.
        /// </summary>
        public bool IsOpening => MediaCore?.State.IsOpening ?? default;

        /// <summary>
        /// Gets a value indicating whether the media is in the process of closing.
        /// </summary>
        public bool IsClosing => MediaCore?.State.IsClosing ?? default;

        /// <summary>
        /// Gets a value indicating whether the media is currently changing its components.
        /// </summary>
        public bool IsChanging => MediaCore?.State.IsChanging ?? default;

        /// <summary>
        /// Gets a value indicating whether this media element
        /// currently has an open media url.
        /// </summary>
        public bool IsOpen => MediaCore?.State.IsOpen ?? default;

        /// <summary>
        /// Gets a value indicating whether the video stream contains closed captions.
        /// </summary>
        public bool HasClosedCaptions => MediaCore?.State.HasClosedCaptions ?? default;
    }


    public partial class MediaElement
    {
        #region IsDesignPreviewEnabled Dependency Property

        /// <summary>
        /// Gets or sets a value that indicates whether the MediaElement will display
        /// a preview image in design time. This is a dependency property.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Gets or sets a value that indicates whether the MediaElement will display a preview image in design time.")]
        public bool IsDesignPreviewEnabled
        {
            get => (bool)GetValue(IsDesignPreviewEnabledProperty);
            set => SetValue(IsDesignPreviewEnabledProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.IsDesignPreviewEnabled property.
        /// </summary>
        public static readonly DependencyProperty IsDesignPreviewEnabledProperty = DependencyProperty.Register(
            nameof(IsDesignPreviewEnabled), typeof(bool), typeof(MediaElement),
            new FrameworkPropertyMetadata(true, OnIsDesignPreviewEnabledPropertyChanged));

        private static void OnIsDesignPreviewEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null || d is MediaElement == false)
                return;

            if (!Library.IsInDesignMode)
                return;

            var element = (MediaElement)d;
            if (element.VideoView == null) return;

            if ((bool)e.NewValue)
            {
                if (element.VideoView.Source == null)
                {
                    var bitmap = Properties.Resources.FFmpegMediaElementBackground;
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    var controlBitmap = new WriteableBitmap(bitmapSource);
                    element.VideoView.Source = controlBitmap;
                }

                element.CaptionsView.Visibility = Visibility.Visible;
                element.SubtitlesView.Visibility = Visibility.Visible;
            }
            else
            {
                element.VideoView.Source = null;
                element.CaptionsView.Visibility = Visibility.Collapsed;
                element.SubtitlesView.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Volume Dependency Property

        /// <summary>
        /// Gets/Sets the Volume property on the MediaElement.
        /// Note: Valid values are from 0 to 1.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("The playback volume. Ranges from 0.0 to 1.0")]
        public double Volume
        {
            get => (double)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.Volume property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            nameof(Volume), typeof(double), typeof(MediaElement),
            new FrameworkPropertyMetadata(Constants.DefaultVolume, null, OnVolumePropertyChanging));

        private static object OnVolumePropertyChanging(DependencyObject d, object value)
        {
            if (d == null || d is MediaElement == false)
                return Constants.DefaultVolume;

            var element = (MediaElement)d;
            if (element.IsStateUpdating)
                return value;

            element.MediaCore.State.Volume = (double)value;
            return element.MediaCore.State.Volume;
        }

        #endregion

        #region Balance Dependency Property

        /// <summary>
        /// Gets/Sets the Balance property on the MediaElement.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("The audio balance for left and right audio channels. Valid ranges are -1.0 to 1.0")]
        public double Balance
        {
            get => (double)GetValue(BalanceProperty);
            set => SetValue(BalanceProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.Balance property.
        /// </summary>
        public static readonly DependencyProperty BalanceProperty = DependencyProperty.Register(
            nameof(Balance), typeof(double), typeof(MediaElement),
            new FrameworkPropertyMetadata(Constants.DefaultBalance, null, OnBalancePropertyChanging));

        private static object OnBalancePropertyChanging(DependencyObject d, object value)
        {
            if (d == null || d is MediaElement == false)
                return Constants.DefaultBalance;

            var element = (MediaElement)d;
            if (element.IsStateUpdating)
                return value;

            element.MediaCore.State.Balance = (double)value;
            return element.MediaCore.State.Balance;
        }

        #endregion

        #region IsMuted Dependency Property

        /// <summary>
        /// Gets/Sets the IsMuted property on the MediaElement.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Gets or sets whether audio samples should be rendered.")]
        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.IsMuted property.
        /// </summary>
        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register(
            nameof(IsMuted), typeof(bool), typeof(MediaElement),
            new FrameworkPropertyMetadata(false, null, OnIsMutedPropertyChanging));

        private static object OnIsMutedPropertyChanging(DependencyObject d, object value)
        {
            if (d == null || d is MediaElement == false)
                return false;

            var element = (MediaElement)d;
            if (element.IsStateUpdating)
                return value;

            element.MediaCore.State.IsMuted = (bool)value;
            return element.MediaCore.State.IsMuted;
        }

        #endregion

        #region SpeedRatio Dependency Property

        /// <summary>
        /// Gets/Sets the SpeedRatio property on the MediaElement.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Specifies how quickly or how slowly the media should be rendered. 1.0 is normal speed. Value must be greater then or equal to 0.0")]
        public double SpeedRatio
        {
            get => (double)GetValue(SpeedRatioProperty);
            set => SetValue(SpeedRatioProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.SpeedRatio property.
        /// </summary>
        public static readonly DependencyProperty SpeedRatioProperty = DependencyProperty.Register(
            nameof(SpeedRatio), typeof(double), typeof(MediaElement),
            new FrameworkPropertyMetadata(Constants.DefaultSpeedRatio, null, OnSpeedRatioPropertyChanging));

        private static object OnSpeedRatioPropertyChanging(DependencyObject d, object value)
        {
            if (d == null || d is MediaElement == false)
                return Constants.DefaultSpeedRatio;

            var element = (MediaElement)d;
            if (element.IsStateUpdating)
                return value;

            element.MediaCore.State.SpeedRatio = (double)value;
            return element.MediaCore.State.SpeedRatio;
        }

        #endregion

        #region Source Dependency Property

        /// <summary>
        /// Gets/Sets the Source on this MediaElement.
        /// The Source property is the Uri of the media to be played.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("The URL to load the media from. Set it to null in order to close the currently open media.")]
        public Uri Source
        {
            get => GetValue(SourceProperty) as Uri;
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// DependencyProperty for FFmpegMediaElement Source property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(Uri), typeof(MediaElement),
            new FrameworkPropertyMetadata(null, OnSourcePropertyChanging));

        private static object OnSourcePropertyChanging(DependencyObject d, object value)
        {
            if (d == null || d is MediaElement == false)
                return null;

            var element = (MediaElement)d;
            if (element.IsStateUpdating)
                return value;

            var uri = value as Uri;
            var mediaCore = element.MediaCore;
            if (mediaCore == null) return null;

            if (uri == null)
            {
                mediaCore.Close();
                return null;
            }

            mediaCore.Open(uri);
            return uri;
        }

        #endregion

        #region Position Dependency Property

        /// <summary>
        /// Gets/Sets the Position property on the MediaElement.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Specifies the position of the underlying media. Set this property to seek though the media stream.")]
        public TimeSpan Position
        {
            get => (TimeSpan)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.Position property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position), typeof(TimeSpan), typeof(MediaElement),
            new FrameworkPropertyMetadata(TimeSpan.Zero,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                null, OnPositionPropertyChanging));

        private static object OnPositionPropertyChanging(DependencyObject d, object value)
        {
            if (d == null || d is MediaElement == false) return value;

            var element = (MediaElement)d;
            var mediaCore = element.MediaCore;

            if (mediaCore == null || mediaCore.IsDisposed || mediaCore.MediaInfo == null || !element.IsOpen)
                return TimeSpan.Zero;

            if (element.IsSeekable == false || element.IsStateUpdating)
                return value;

            // Clamp from minimum to maximum
            var targetSeek = (TimeSpan)value;
            var minTarget = mediaCore.State.PlaybackStartTime ?? TimeSpan.Zero;
            var maxTarget = mediaCore.State.PlaybackEndTime ?? TimeSpan.Zero;
            var hasValidTaget = maxTarget > minTarget;

            if (hasValidTaget)
            {
                targetSeek = targetSeek.Clamp(minTarget, maxTarget);
                mediaCore?.Seek(targetSeek);
            }
            else
            {
                targetSeek = mediaCore.State.Position;
            }

            return targetSeek;
        }

        #endregion

        #region ScrubbingEnabled Dependency Property

        /// <summary>
        /// Gets or sets a value that indicates whether the MediaElement will update frames
        /// for seek operations while paused. This is a dependency property.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Gets or sets a value that indicates whether the MediaElement will update frames for seek operations while paused.")]
        public bool ScrubbingEnabled
        {
            get => (bool)GetValue(ScrubbingEnabledProperty);
            set => SetValue(ScrubbingEnabledProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.ScrubbingEnabled property.
        /// </summary>
        public static readonly DependencyProperty ScrubbingEnabledProperty = DependencyProperty.Register(
            nameof(ScrubbingEnabled), typeof(bool), typeof(MediaElement),
            new FrameworkPropertyMetadata(true));

        #endregion

        #region LoadedBahavior Dependency Property

        /// <summary>
        /// Specifies the action that the media element should execute when it
        /// is loaded. The default behavior is that it is under manual control
        /// (i.e. the caller should call methods such as Play in order to play
        /// the media). If a source is set, then the default behavior changes to
        /// to be playing the media. If a source is set and a loaded behavior is
        /// also set, then the loaded behavior takes control.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Specifies how the underlying media should behave when it has loaded. The default behavior is to Play the media.")]
        public MediaPlaybackState LoadedBehavior
        {
            get => (MediaPlaybackState)GetValue(LoadedBehaviorProperty);
            set => SetValue(LoadedBehaviorProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.LoadedBehavior property.
        /// </summary>
        public static readonly DependencyProperty LoadedBehaviorProperty = DependencyProperty.Register(
            nameof(LoadedBehavior), typeof(MediaPlaybackState), typeof(MediaElement),
            new FrameworkPropertyMetadata(MediaPlaybackState.Manual));

        #endregion

        #region UnoadedBahavior Dependency Property

        /// <summary>
        /// Specifies how the underlying media engine's resources should be handled when
        /// the unloaded event gets fired. The default behavior is to Close and release the resources.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Specifies how the underlying media engine's resources should be handled when the unloaded event gets fired.")]
        public MediaPlaybackState UnloadedBehavior
        {
            get => (MediaPlaybackState)GetValue(UnloadedBehaviorProperty);
            set => SetValue(UnloadedBehaviorProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.UnloadedBehavior property.
        /// </summary>
        public static readonly DependencyProperty UnloadedBehaviorProperty = DependencyProperty.Register(
            nameof(UnloadedBehavior), typeof(MediaPlaybackState), typeof(MediaElement),
            new FrameworkPropertyMetadata(MediaPlaybackState.Close, OnUnloadedBehaviorPropertyChanged));

        private static void OnUnloadedBehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null || d is MediaElement == false)
                return;

            var element = (MediaElement)d;
            if (element.VideoView == null) return;

            var behavior = element.UnloadedBehavior;
            element.VideoView.PreventShutdown = behavior != MediaPlaybackState.Close;
        }

        #endregion

        #region LoopingBehavior Dependency Property

        /// <summary>
        /// Specifies how the media should behave when it has ended. The default behavior is to Pause the media.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("Specifies how the media should behave when it has ended. The default behavior is to Pause the media.")]
        public MediaPlaybackState LoopingBehavior
        {
            get => (MediaPlaybackState)GetValue(LoopingBehaviorProperty);
            set => SetValue(LoopingBehaviorProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.LoopingBehavior property.
        /// </summary>
        public static readonly DependencyProperty LoopingBehaviorProperty = DependencyProperty.Register(
            nameof(LoopingBehavior), typeof(MediaPlaybackState), typeof(MediaElement),
            new FrameworkPropertyMetadata(MediaPlaybackState.Pause));

        #endregion

        #region ClosedCaptionsChannel Dependency Property

        /// <summary>
        /// Gets/Sets the ClosedCaptionsChannel property on the MediaElement.
        /// Note: Valid values are from 0 to 1.
        /// </summary>
        [Category(nameof(MediaElement))]
        [Description("The video CC Channel to render. Ranges from 0 to 4")]
        public CaptionsChannel ClosedCaptionsChannel
        {
            get => (CaptionsChannel)GetValue(ClosedCaptionsChannelProperty);
            set => SetValue(ClosedCaptionsChannelProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the MediaElement.ClosedCaptionsChannel property.
        /// </summary>
        public static readonly DependencyProperty ClosedCaptionsChannelProperty = DependencyProperty.Register(
            nameof(ClosedCaptionsChannel), typeof(CaptionsChannel), typeof(MediaElement),
            new FrameworkPropertyMetadata(Constants.DefaultClosedCaptionsChannel, OnClosedCaptionsChannelPropertyChanged));

        private static void OnClosedCaptionsChannelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MediaElement m) m.CaptionsView.Reset();
        }

        #endregion

        #region Stretch Dependency Property

        /// <summary>
        /// Gets/Sets the Stretch on this MediaElement.
        /// The Stretch property determines how large the MediaElement will be drawn.
        /// </summary>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// DependencyProperty for Stretch property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            nameof(Stretch), typeof(Stretch), typeof(MediaElement),
            new FrameworkPropertyMetadata(Stretch.Uniform, AffectsMeasureAndRender, OnStretchPropertyChanged));

        private static void OnStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MediaElement m && m.VideoView != null && m.VideoView.IsLoaded && e.NewValue is Stretch v)
                m.VideoView.Stretch = v;
        }

        #endregion

        #region StretchDirection Dependency Property

        /// <summary>
        /// Gets/Sets the stretch direction of the ViewBox, which determines the restrictions on
        /// scaling that are applied to the content inside the ViewBox.  For instance, this property
        /// can be used to prevent the content from being smaller than its native size or larger than
        /// its native size.
        /// </summary>
        public StretchDirection StretchDirection
        {
            get => (StretchDirection)GetValue(StretchDirectionProperty);
            set => SetValue(StretchDirectionProperty, value);
        }

        /// <summary>
        /// DependencyProperty for StretchDirection property.
        /// </summary>
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register(
            nameof(StretchDirection), typeof(StretchDirection), typeof(MediaElement),
            new FrameworkPropertyMetadata(StretchDirection.Both, AffectsMeasureAndRender, OnStretchDirectionPropertyChanged));

        private static void OnStretchDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MediaElement m && m.VideoView != null && m.VideoView.IsLoaded && e.NewValue is StretchDirection v)
                m.VideoView.StretchDirection = v;
        }

        #endregion
    }
}
