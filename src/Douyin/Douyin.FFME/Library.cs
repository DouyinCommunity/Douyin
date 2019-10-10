﻿namespace Douyin.FFME
{
    using Common;
    using Container;
    using Douyin.FFME.Platform;
    using Douyin.FFME.Rendering.Wave;
    using FFmpeg.AutoGen;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Provides access to the underlying FFmpeg library information.
    /// </summary>
    public static partial class Library
    {
        private static readonly string NotInitializedErrorMessage =
            $"{nameof(FFmpeg)} library not initialized. Set the {nameof(FFmpegDirectory)} and call {nameof(LoadFFmpeg)}";

        private static readonly object SyncLock = new object();
        private static string m_FFmpegDirectory = Constants.FFmpegSearchPath;
        private static int m_FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;
        private static IReadOnlyList<string> m_InputFormatNames;
        private static IReadOnlyList<OptionMetadata> m_GlobalInputFormatOptions;
        private static IReadOnlyDictionary<string, IReadOnlyList<OptionMetadata>> m_InputFormatOptions;
        private static IReadOnlyList<string> m_DecoderNames;
        private static IReadOnlyList<string> m_EncoderNames;
        private static IReadOnlyList<OptionMetadata> m_GlobalDecoderOptions;
        private static IReadOnlyDictionary<string, IReadOnlyList<OptionMetadata>> m_DecoderOptions;
        private static unsafe AVCodec*[] m_AllCodecs;
        private static int m_FFmpegLogLevel = Debugger.IsAttached ? ffmpeg.AV_LOG_VERBOSE : ffmpeg.AV_LOG_WARNING;

        /// <summary>
        /// Gets or sets the FFmpeg path from which to load the FFmpeg binaries.
        /// You must set this path before setting the Source property for the first time on any instance of this control.
        /// Setting this property when FFmpeg binaries have been registered will have no effect.
        /// </summary>
        public static string FFmpegDirectory
        {
            get => m_FFmpegDirectory;
            set
            {
                if (FFInterop.IsInitialized)
                    return;

                m_FFmpegDirectory = value;
            }
        }

        /// <summary>
        /// Gets the FFmpeg version information. Returns null
        /// when the libraries have not been loaded.
        /// </summary>
        public static string FFmpegVersionInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the bitwise library identifiers to load.
        /// See the <see cref="FFmpegLoadMode"/> constants.
        /// If FFmpeg is already loaded, the value cannot be changed.
        /// </summary>
        public static int FFmpegLoadModeFlags
        {
            get => m_FFmpegLoadModeFlags;
            set
            {
                if (FFInterop.IsInitialized)
                    return;

                m_FFmpegLoadModeFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the FFmpeg log level.
        /// </summary>
        public static int FFmpegLogLevel
        {
            get
            {
                return IsInitialized
                    ? ffmpeg.av_log_get_level()
                    : m_FFmpegLogLevel;
            }
            set
            {
                if (IsInitialized) ffmpeg.av_log_set_level(value);
                m_FFmpegLogLevel = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the FFmpeg library has been initialized.
        /// </summary>
        public static bool IsInitialized => FFInterop.IsInitialized;

        /// <summary>
        /// Gets the registered FFmpeg input format names.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static IReadOnlyList<string> InputFormatNames
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    return m_InputFormatNames ?? (m_InputFormatNames = FFInterop.RetrieveInputFormatNames());
                }
            }
        }

        /// <summary>
        /// Gets the global input format options information.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static IReadOnlyList<OptionMetadata> InputFormatOptionsGlobal
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    return m_GlobalInputFormatOptions ?? (m_GlobalInputFormatOptions = FFInterop.RetrieveGlobalFormatOptions());
                }
            }
        }

        /// <summary>
        /// Gets the input format options.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static IReadOnlyDictionary<string, IReadOnlyList<OptionMetadata>> InputFormatOptions
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    if (m_InputFormatOptions != null)
                        return m_InputFormatOptions;

                    var result = new Dictionary<string, IReadOnlyList<OptionMetadata>>(InputFormatNames.Count);
                    foreach (var formatName in InputFormatNames)
                    {
                        var optionsInfo = FFInterop.RetrieveInputFormatOptions(formatName);
                        result[formatName] = optionsInfo;
                    }

                    m_InputFormatOptions = new Dictionary<string, IReadOnlyList<OptionMetadata>>(result);

                    return m_InputFormatOptions;
                }
            }
        }

        /// <summary>
        /// Gets the registered FFmpeg decoder codec names.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static unsafe IReadOnlyList<string> DecoderNames
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    return m_DecoderNames ?? (m_DecoderNames = FFInterop.RetrieveDecoderNames(AllCodecs));
                }
            }
        }

        /// <summary>
        /// Gets the registered FFmpeg decoder codec names.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static unsafe IReadOnlyList<string> EncoderNames
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    return m_EncoderNames ?? (m_EncoderNames = FFInterop.RetrieveEncoderNames(AllCodecs));
                }
            }
        }

        /// <summary>
        /// Gets the global options that apply to all decoders.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static IReadOnlyList<OptionMetadata> DecoderOptionsGlobal
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    return m_GlobalDecoderOptions ??
                        (m_GlobalDecoderOptions = FFInterop.RetrieveGlobalCodecOptions().Where(o => o.IsDecodingOption).ToArray());
                }
            }
        }

        /// <summary>
        /// Gets the decoder specific options.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        public static unsafe IReadOnlyDictionary<string, IReadOnlyList<OptionMetadata>> DecoderOptions
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    if (m_DecoderOptions != null)
                        return m_DecoderOptions;

                    var result = new Dictionary<string, IReadOnlyList<OptionMetadata>>(DecoderNames.Count);
                    foreach (var c in AllCodecs)
                    {
                        if (c->decode.Pointer == IntPtr.Zero)
                            continue;

                        result[Utilities.PtrToStringUTF8(c->name)] = FFInterop.RetrieveCodecOptions(c);
                    }

                    m_DecoderOptions = new Dictionary<string, IReadOnlyList<OptionMetadata>>(result);

                    return m_DecoderOptions;
                }
            }
        }

        /// <summary>
        /// Gets all registered encoder and decoder codecs.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the MediaEngine has not been initialized.</exception>
        internal static unsafe AVCodec*[] AllCodecs
        {
            get
            {
                lock (SyncLock)
                {
                    if (!FFInterop.IsInitialized)
                        throw new InvalidOperationException(NotInitializedErrorMessage);

                    return m_AllCodecs ?? (m_AllCodecs = FFInterop.RetrieveCodecs());
                }
            }
        }

        /// <summary>
        /// Forces the pre-loading of the FFmpeg libraries according to the values of the
        /// <see cref="FFmpegDirectory"/> and <see cref="FFmpegLoadModeFlags"/>
        /// Also, sets the <see cref="FFmpegVersionInfo"/> property. Throws an exception
        /// if the libraries cannot be loaded.
        /// </summary>
        /// <returns>true if libraries were loaded, false if libraries were already loaded.</returns>
        public static bool LoadFFmpeg()
        {
            if (!FFInterop.Initialize(FFmpegDirectory, FFmpegLoadModeFlags))
                return false;

            // Set the folders and lib identifiers
            FFmpegDirectory = FFInterop.LibrariesPath;
            FFmpegLoadModeFlags = FFInterop.LibraryIdentifiers;
            FFmpegVersionInfo = ffmpeg.av_version_info();
            return true;
        }

        /// <summary>
        /// Provides an asynchronous version of the <see cref="LoadFFmpeg"/> call.
        /// </summary>
        /// <returns>true if libraries were loaded, false if libraries were already loaded.</returns>
        public static async Task<bool> LoadFFmpegAsync() =>
            await Task.Run(() => LoadFFmpeg()).ConfigureAwait(true);

        /// <summary>
        /// Unloads FFmpeg libraries from memory.
        /// </summary>
        /// <exception cref="NotImplementedException">Unloading FFmpeg libraries is not yet supported.</exception>
        public static void UnloadFFmpeg() =>
            throw new NotImplementedException("Unloading FFmpeg libraries is not yet supported");

        /// <summary>
        /// Retrieves the media information including all streams, chapters and programs.
        /// </summary>
        /// <param name="mediaSource">The source URL.</param>
        /// <returns>The contents of the media information.</returns>
        public static MediaInfo RetrieveMediaInfo(string mediaSource)
        {
            using (var container = new MediaContainer(mediaSource, null, null))
            {
                container.Initialize();
                return container.MediaInfo;
            }
        }

        /// <summary>
        /// Creates a viedo seek index object.
        /// </summary>
        /// <param name="mediaSource">The source URL.</param>
        /// <param name="streamIndex">Index of the stream. Use -1 for automatic stream selection.</param>
        /// <returns>
        /// The seek index object.
        /// </returns>
        public static VideoSeekIndex CreateVideoSeekIndex(string mediaSource, int streamIndex)
        {
            var result = new VideoSeekIndex(mediaSource, -1);

            using (var container = new MediaContainer(mediaSource, null, null))
            {
                container.Initialize();
                container.MediaOptions.IsAudioDisabled = true;
                container.MediaOptions.IsVideoDisabled = false;
                container.MediaOptions.IsSubtitleDisabled = true;

                if (streamIndex >= 0)
                    container.MediaOptions.VideoStream = container.MediaInfo.Streams[streamIndex];

                container.Open();
                result.StreamIndex = container.Components.Video.StreamIndex;
                while (container.IsStreamSeekable)
                {
                    container.Read();
                    var frames = container.Decode();
                    foreach (var frame in frames)
                    {
                        try
                        {
                            if (frame.MediaType != MediaType.Video)
                                continue;

                            // Check if the frame is a key frame and add it to the index.
                            result.TryAdd(frame as VideoFrame);
                        }
                        finally
                        {
                            frame.Dispose();
                        }
                    }

                    // We have reached the end of the stream.
                    if (frames.Count <= 0 && container.IsAtEndOfStream)
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a viedo seek index object of the default video stream.
        /// </summary>
        /// <param name="mediaSource">The source URL.</param>
        /// <returns>
        /// The seek index object.
        /// </returns>
        public static VideoSeekIndex CreateVideoSeekIndex(string mediaSource) => CreateVideoSeekIndex(mediaSource, -1);
    }

    public static partial class Library
    {
        private static IGuiContext m_GuiContext;

        /// <summary>
        /// Provides access to the registered GUI context.
        /// </summary>
        internal static IGuiContext GuiContext
        {
            get
            {
                lock (SyncLock)
                {
                    if (m_GuiContext == null)
                        throw new InvalidOperationException($"{nameof(IGuiContext)} has not been registered.");

                    return m_GuiContext;
                }
            }
        }

        /// <summary>
        /// Registers the GUI context for the library.
        /// </summary>
        /// <param name="context">The GUI context to register.</param>
        internal static void RegisterGuiContext(IGuiContext context)
        {
            lock (SyncLock)
            {
                if (m_GuiContext != null)
                    throw new InvalidOperationException($"{nameof(GuiContext)} has already been registered.");

                m_GuiContext = context ?? throw new ArgumentNullException($"{nameof(context)} cannot be null.");
            }
        }
    }

    public static partial class Library
    {
        /// <summary>
        /// Gets or sets a value indicating whether the video visualization control
        /// creates its own dispatcher thread to handle rendering of video frames.
        /// This is an experimental feature and it is useful when creating video walls.
        /// For example if you want to display multiple videos at a time and don't want to
        /// use time from the main UI thread. This feature is only valid if we are in
        /// a WPF context.
        /// </summary>
        public static bool EnableWpfMultiThreadedVideo { get; set; }

        /// <summary>
        /// The default DirectSound device.
        /// </summary>
        public static DirectSoundDeviceInfo DefaultDirectSoundDevice { get; } = new DirectSoundDeviceInfo(
            DirectSoundPlayer.DefaultPlaybackDeviceId, nameof(DefaultDirectSoundDevice), nameof(DirectSoundPlayer), true, Guid.Empty.ToString());

        /// <summary>
        /// The default Windows Multimeda Extensions Legacy Audio Device.
        /// </summary>
        public static LegacyAudioDeviceInfo DefaultLegacyAudioDevice { get; } = new LegacyAudioDeviceInfo(
            -1, nameof(DefaultLegacyAudioDevice), nameof(LegacyAudioPlayer), true, Guid.Empty.ToString());

        /// <summary>
        /// Determines if the control library is currently in design-time mode (as opposed to run-time).
        /// </summary>
        internal static bool IsInDesignMode =>
            (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        /// <summary>
        /// Enumerates the DirectSound devices.
        /// </summary>
        /// <returns>The available DirectSound devices.</returns>
        public static IEnumerable<DirectSoundDeviceInfo> EnumerateDirectSoundDevices()
        {
            var devices = DirectSoundPlayer.EnumerateDevices();
            var result = new List<DirectSoundDeviceInfo>(16) { DefaultDirectSoundDevice };

            foreach (var device in devices)
            {
                result.Add(new DirectSoundDeviceInfo(
                    device.Guid, device.Description, nameof(DirectSoundPlayer), false, device.ModuleName));
            }

            return result;
        }

        /// <summary>
        /// Enumerates the (Legacy) Windows Multimedia Extensions devices.
        /// </summary>
        /// <returns>The available MME devices.</returns>
        public static IEnumerable<LegacyAudioDeviceInfo> EnumerateLegacyAudioDevices()
        {
            var devices = LegacyAudioPlayer.EnumerateDevices();
            var result = new List<LegacyAudioDeviceInfo>(16) { DefaultLegacyAudioDevice };

            for (var deviceId = 0; deviceId < devices.Count; deviceId++)
            {
                var device = devices[deviceId];
                result.Add(new LegacyAudioDeviceInfo(
                    deviceId, device.ProductName, nameof(LegacyAudioPlayer), false, device.ProductGuid.ToString()));
            }

            return result;
        }
    }
}
