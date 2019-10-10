namespace Douyin.FFME
{
    using Common;
    using Diagnostics;
    using Douyin.FFME.Container;
    using FFmpeg.AutoGen;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public partial class MediaElement
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a logging message has been logged.
        /// This does not include FFmpeg messages.
        /// </summary>
        /// <remarks>
        /// This event is raised on a background thread.
        /// All interaction with UI elements requires calls on their corresponding dispatcher.
        /// </remarks>
        public event EventHandler<MediaLogMessageEventArgs> MessageLogged;

        /// <summary>
        /// Raised before the input stream of the media is initialized.
        /// Use this method to modify the input options.
        /// </summary>
        /// <remarks>
        /// This event is raised on a background thread.
        /// All interaction with UI elements requires calls on their corresponding dispatcher.
        /// </remarks>
        public event EventHandler<MediaInitializingEventArgs> MediaInitializing;

        /// <summary>
        /// Raised before the input stream of the media is opened.
        /// Use this method to modify the media options and select streams.
        /// </summary>
        /// <remarks>
        /// This event is raised on a background thread.
        /// All interaction with UI elements requires calls on their corresponding dispatcher.
        /// </remarks>
        public event EventHandler<MediaOpeningEventArgs> MediaOpening;

        /// <summary>
        /// Raised before a change in media options is applied.
        /// Use this method to modify the selected streams.
        /// </summary>
        /// <remarks>
        /// This event is raised on a background thread.
        /// All interaction with UI elements requires calls on their corresponding dispatcher.
        /// </remarks>
        public event EventHandler<MediaOpeningEventArgs> MediaChanging;

        /// <summary>
        /// Raised when a packet is read from the input stream. Useful for capturing streams.
        /// This event is not raised on the UI thread and the pointers in the event arguments
        /// are only valid for the call. If you need to keep a queue you will need to clone and
        /// release the allocated memory yourself by using clone and release methods in the native
        /// FFmpeg API.
        /// </summary>
        public event EventHandler<PacketReadEventArgs> PacketRead;

        /// <summary>
        /// Raised when an audio frame is decoded from input stream. Useful for capturing streams.
        /// This event is not raised on the UI thread and the pointers in the event arguments
        /// are only valid for the call. If you need to keep a queue you will need to clone and
        /// release the allocated memory yourself by using clone and release methods in the native
        /// FFmpeg API.
        /// </summary>
        public event EventHandler<FrameDecodedEventArgs> AudioFrameDecoded;

        /// <summary>
        /// Raised when a video frame is decoded from input stream. Useful for capturing streams.
        /// This event is not raised on the UI thread and the pointers in the event arguments
        /// are only valid for the call. If you need to keep a queue you will need to clone and
        /// release the allocated memory yourself by using clone and release methods in the native
        /// FFmpeg API.
        /// </summary>
        public event EventHandler<FrameDecodedEventArgs> VideoFrameDecoded;

        /// <summary>
        /// Raised when a subtitle is decoded from input stream. Useful for capturing streams.
        /// This event is not raised on the UI thread and the pointers in the event arguments
        /// are only valid for the call. If you need to keep a queue you will need to clone and
        /// release the allocated memory yourself by using clone and release methods in the native
        /// FFmpeg API.
        /// </summary>
        public event EventHandler<SubtitleDecodedEventArgs> SubtitleDecoded;

        /// <summary>
        /// Occurs when buffering of packets was started
        /// </summary>
        public event EventHandler BufferingStarted;

        /// <summary>
        /// Occurs when buffering of packets has ended
        /// </summary>
        public event EventHandler BufferingEnded;

        /// <summary>
        /// Occurs when stream seeking has started
        /// </summary>
        public event EventHandler SeekingStarted;

        /// <summary>
        /// Occurs when Seeking of packets was Ended
        /// </summary>
        public event EventHandler SeekingEnded;

        /// <summary>
        /// Raised when the media is opened
        /// </summary>
        public event EventHandler<MediaOpenedEventArgs> MediaOpened;

        /// <summary>
        /// Raised after the media is opened and ready to receive commands
        /// such as <see cref="Seek(TimeSpan)"/>
        /// </summary>
        public event EventHandler MediaReady;

        /// <summary>
        /// Raised after a change in media options and components is applied.
        /// </summary>
        public event EventHandler<MediaOpenedEventArgs> MediaChanged;

        /// <summary>
        /// Raised when the media is closed
        /// </summary>
        public event EventHandler MediaClosed;

        /// <summary>
        /// Raised when the corresponding media ends.
        /// </summary>
        public event EventHandler MediaEnded;

        /// <summary>
        /// Occurs when media position is changed
        /// </summary>
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        /// <summary>
        /// Occurs when media state is changed
        /// </summary>
        public event EventHandler<MediaStateChangedEventArgs> MediaStateChanged;

        /// <summary>
        /// Raised when the media fails to load or a fatal error has occurred which prevents playback.
        /// </summary>
        public event EventHandler<MediaFailedEventArgs> MediaFailed;

        #region Non-UI event raisers

        /// <summary>
        /// Raises the message logged event.
        /// </summary>
        /// <param name="e">The <see cref="LoggingMessage"/> instance containing the event data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseMessageLoggedEvent(LoggingMessage e) =>
            MessageLogged?.Invoke(this, new MediaLogMessageEventArgs(e));

        /// <summary>
        /// Raises the media initializing event.
        /// </summary>
        /// <param name="config">The container configuration options.</param>
        /// <param name="url">The URL.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseMediaInitializingEvent(ContainerConfiguration config, string url) =>
            MediaInitializing?.Invoke(this, new MediaInitializingEventArgs(config, url));

        /// <summary>
        /// Raises the media opening event.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mediaInfo">The media information.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseMediaOpeningEvent(MediaOptions options, MediaInfo mediaInfo) =>
            MediaOpening?.Invoke(this, new MediaOpeningEventArgs(options, mediaInfo));

        /// <summary>
        /// Raises the media changing event.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mediaInfo">The media information.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseMediaChangingEvent(MediaOptions options, MediaInfo mediaInfo) =>
            MediaChanging?.Invoke(this, new MediaOpeningEventArgs(options, mediaInfo));

        /// <summary>
        /// Raises the packet read event.
        /// </summary>
        /// <param name="packet">The packet pointer.</param>
        /// <param name="context">The input context pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe void RaisePacketReadEvent(AVPacket* packet, AVFormatContext* context) =>
            PacketRead?.Invoke(this, new PacketReadEventArgs(packet, context));

        /// <summary>
        /// Raises the audio frame decoded event.
        /// </summary>
        /// <param name="frame">The frame pointer.</param>
        /// <param name="context">The input context pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe void RaiseAudioFrameDecodedEvent(AVFrame* frame, AVFormatContext* context) =>
            AudioFrameDecoded?.Invoke(this, new FrameDecodedEventArgs(frame, context));

        /// <summary>
        /// Raises the video frame decoded event.
        /// </summary>
        /// <param name="frame">The frame pointer.</param>
        /// <param name="context">The input context pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe void RaiseVideoFrameDecodedEvent(AVFrame* frame, AVFormatContext* context) =>
            VideoFrameDecoded?.Invoke(this, new FrameDecodedEventArgs(frame, context));

        /// <summary>
        /// Raises the subtitle decoded event.
        /// </summary>
        /// <param name="subtitle">The subtitle pointer.</param>
        /// <param name="context">The input context pointer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe void RaiseSubtitleDecodedEvent(AVSubtitle* subtitle, AVFormatContext* context) =>
            SubtitleDecoded?.Invoke(this, new SubtitleDecodedEventArgs(subtitle, context));

        #endregion

        #region UI Context Event Raisers

        /// <summary>
        /// Raises the media failed event.
        /// </summary>
        /// <param name="ex">The ex.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaFailedEvent(Exception ex)
        {
            LogEventStart(nameof(MediaFailed));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaFailed?.Invoke(this, new MediaFailedEventArgs(ex));
                LogEventDone(nameof(MediaFailed));
            });
        }

        /// <summary>
        /// Raises the media opened event.
        /// </summary>
        /// <param name="mediaInfo">The media information.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaOpenedEvent(MediaInfo mediaInfo)
        {
            LogEventStart(nameof(MediaOpened));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaOpened?.Invoke(this, new MediaOpenedEventArgs(mediaInfo));
                LogEventDone(nameof(MediaOpened));
            });
        }

        /// <summary>
        /// Raises the media ready event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaReadyEvent()
        {
            LogEventStart(nameof(MediaReady));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaReady?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(MediaReady));
            });
        }

        /// <summary>
        /// Raises the media closed event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaClosedEvent()
        {
            LogEventStart(nameof(MediaClosed));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaClosed?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(MediaClosed));
            });
        }

        /// <summary>
        /// Raises the media changed event.
        /// </summary>
        /// <param name="mediaInfo">The media information.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaChangedEvent(MediaInfo mediaInfo)
        {
            LogEventStart(nameof(MediaChanged));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaChanged?.Invoke(this, new MediaOpenedEventArgs(mediaInfo));
                LogEventDone(nameof(MediaChanged));
            });
        }

        /// <summary>
        /// Raises the position changed event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostPositionChangedEvent(TimeSpan oldValue, TimeSpan newValue)
        {
            // Event logging disabled because this happens too often.
            Library.GuiContext.EnqueueInvoke(() =>
            {
                PositionChanged?.Invoke(
                    this, new PositionChangedEventArgs(MediaCore.State, oldValue, newValue));
            });
        }

        /// <summary>
        /// Raises the media state changed event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaStateChangedEvent(MediaPlaybackState oldValue, MediaPlaybackState newValue)
        {
            LogEventStart(nameof(MediaStateChanged));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaStateChanged?.Invoke(this, new MediaStateChangedEventArgs(oldValue, newValue));
                LogEventDone(nameof(MediaStateChanged));
            });
        }

        /// <summary>
        /// Raises the buffering started event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostBufferingStartedEvent()
        {
            LogEventStart(nameof(BufferingStarted));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                BufferingStarted?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(BufferingStarted));
            });
        }

        /// <summary>
        /// Raises the buffering ended event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostBufferingEndedEvent()
        {
            LogEventStart(nameof(BufferingEnded));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                BufferingEnded?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(BufferingEnded));
            });
        }

        /// <summary>
        /// Raises the Seeking started event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostSeekingStartedEvent()
        {
            LogEventStart(nameof(SeekingStarted));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                SeekingStarted?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(SeekingStarted));
            });
        }

        /// <summary>
        /// Raises the Seeking ended event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostSeekingEndedEvent()
        {
            LogEventStart(nameof(SeekingEnded));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                SeekingEnded?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(SeekingEnded));
            });
        }

        /// <summary>
        /// Raises the media ended event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void PostMediaEndedEvent()
        {
            LogEventStart(nameof(MediaEnded));
            Library.GuiContext.EnqueueInvoke(() =>
            {
                MediaEnded?.Invoke(this, EventArgs.Empty);
                LogEventDone(nameof(MediaEnded));
            });
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void NotifyPropertyChangedEvent(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Event Logging

        /// <summary>
        /// Logs the start of an event.
        /// </summary>
        /// <param name="eventName">The event.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogEventStart(string eventName)
        {
            if (Debugger.IsAttached)
                this.LogTrace(Aspects.Events, $"EVENT START: {eventName}");
        }

        /// <summary>
        /// Logs the end of an event.
        /// </summary>
        /// <param name="eventName">The event.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogEventDone(string eventName)
        {
            if (Debugger.IsAttached)
                this.LogTrace(Aspects.Events, $"EVENT DONE : {eventName}");
        }

        #endregion
    }

    public partial class MediaElement
    {
        #region Events

        /// <summary>
        /// Occurs when a logging message from the FFmpeg library has been received.
        /// This is shared across all instances of Media Elements.
        /// </summary>
        /// <remarks>
        /// This event is raised on a background thread.
        /// All interaction with UI elements requires calls on their corresponding dispatcher.
        /// </remarks>
        public static event EventHandler<MediaLogMessageEventArgs> FFmpegMessageLogged;

        /// <summary>
        /// Occurs right before the video is presented on the screen.
        /// You can update the pixels on the bitmap before it is rendered on the screen.
        /// Or you could take a screen shot.
        /// Ensure you handle this very quickly as it runs on the UI thread.
        /// </summary>
        public event EventHandler<RenderingVideoEventArgs> RenderingVideo;

        /// <summary>
        /// Occurs right before the audio is added to the audio buffer.
        /// You can update the bytes before they are queued.
        /// Ensure you handle this quickly before you get choppy audio.
        /// </summary>
        public event EventHandler<RenderingAudioEventArgs> RenderingAudio;

        /// <summary>
        /// Occurs right before the subtitles are rendered.
        /// You can update the text.
        /// Ensure you handle this quickly before you get choppy subtitles.
        /// </summary>
        public event EventHandler<RenderingSubtitlesEventArgs> RenderingSubtitles;

        /// <summary>
        /// Occurs when the currently selected audio device stops or loses its buffer.
        /// Call the <see cref="ChangeMedia"/> method and select a new audio device
        /// in order to output to a new audio device
        /// </summary>
        public event EventHandler AudioDeviceStopped;

        #endregion

        #region Event Raisers

        /// <summary>
        /// Raises the rendering video event.
        /// </summary>
        /// <param name="videoBlock">The block.</param>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="clock">The clock.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseRenderingVideoEvent(VideoBlock videoBlock, BitmapDataBuffer bitmap, TimeSpan clock)
        {
            if (RenderingVideo == null) return;

            var e = new RenderingVideoEventArgs(
                bitmap,
                videoBlock.ClosedCaptions,
                videoBlock.SmtpeTimeCode,
                videoBlock.DisplayPictureNumber,
                MediaCore.State,
                MediaCore.MediaInfo.Streams[videoBlock.StreamIndex],
                videoBlock.StartTime,
                videoBlock.Duration,
                clock);

            RenderingVideo?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the rendering audio event.
        /// </summary>
        /// <param name="buffer">The audio buffer.</param>
        /// <param name="bufferLength">Length of the buffer.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="latency">The latency between the current buffer position and the real-time playback clock.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RaiseRenderingAudioEvent(
            byte[] buffer, int bufferLength, TimeSpan startTime, TimeSpan duration, TimeSpan latency)
        {
            if (RenderingAudio == null) return;
            if (MediaCore == null || MediaCore.IsDisposed) return;
            if (MediaCore.MediaInfo.Streams.ContainsKey(MediaCore.State.AudioStreamIndex) == false) return;

            var e = new RenderingAudioEventArgs(
                    buffer,
                    bufferLength,
                    MediaCore.State,
                    MediaCore.MediaInfo.Streams[MediaCore.State.AudioStreamIndex],
                    startTime,
                    duration,
                    MediaCore.PlaybackPosition,
                    latency);

            RenderingAudio?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the rendering subtitles event.
        /// Returning true cancels the rendering of subtitles.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="clock">The clock.</param>
        /// <returns>True if the rendering should be prevented.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool RaiseRenderingSubtitlesEvent(SubtitleBlock block, TimeSpan clock)
        {
            if (RenderingSubtitles == null) return false;

            var e = new RenderingSubtitlesEventArgs(
                    block.Text,
                    block.OriginalText,
                    block.OriginalTextType,
                    MediaCore.State,
                    MediaCore.MediaInfo.Streams[block.StreamIndex],
                    block.StartTime,
                    block.Duration,
                    clock);

            RenderingSubtitles?.Invoke(this, e);
            return e.Cancel;
        }

        /// <summary>
        /// Raises the audio device stopped event.
        /// </summary>
        internal void RaiseAudioDeviceStoppedEvent() =>
            AudioDeviceStopped?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
