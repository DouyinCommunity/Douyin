﻿namespace Douyin.FFME.Windows.ViewModels
{
    using Common;
    using Foundation;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Data;

    /// <summary>
    /// Represents the Playlist.
    /// </summary>
    /// <seealso cref="AttachedViewModel" />
    public sealed class PlaylistViewModel : AttachedViewModel
    {
        #region Private State

        // Constants
        private const int MinimumSearchLength = 3;

        // Private state management
        private readonly TimeSpan SearchActionDelay = TimeSpan.FromSeconds(0.25);
        private bool HasTakenThumbnail;
        private DeferredAction SearchAction;
        private string FilterString = string.Empty;

        // Property Backing
        private bool m_IsInOpenMode = App.IsInDesignMode;
        private bool m_IsPlaylistEnabled = true;
        private string m_OpenMediaSource = string.Empty;
        private string m_PlaylistSearchString = string.Empty;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewModel"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        public PlaylistViewModel(RootViewModel root)
            : base(root)
        {
            // Set and create a thumbnails directory
            ThumbsDirectory = Path.Combine(root.AppDataDirectory, "Thumbnails");
            if (Directory.Exists(ThumbsDirectory) == false)
                Directory.CreateDirectory(ThumbsDirectory);

            // Set and create a index directory
            IndexDirectory = Path.Combine(root.AppDataDirectory, "SeekIndexes");
            if (Directory.Exists(IndexDirectory) == false)
                Directory.CreateDirectory(IndexDirectory);

            PlaylistFilePath = Path.Combine(root.AppDataDirectory, "ffme.m3u8");

            Entries = new CustomPlaylistEntryCollection(this);
            EntriesView = CollectionViewSource.GetDefaultView(Entries);
            EntriesView.Filter = item =>
            {
                var searchString = PlaylistSearchString;

                if (string.IsNullOrWhiteSpace(searchString) || searchString.Trim().Length < MinimumSearchLength)
                    return true;

                if (item is CustomPlaylistEntry entry)
                {
                    var title = entry.Title ?? string.Empty;
                    var source = entry.MediaSource ?? string.Empty;

                    return title.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        source.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
                }

                return false;
            };

            NotifyPropertyChanged(nameof(EntriesView));
        }

        /// <summary>
        /// Gets the custom playlist. Do not use for data-binding.
        /// </summary>
        public CustomPlaylistEntryCollection Entries { get; }

        /// <summary>
        /// Gets the custom playlist entries as a view that can be used in data binding scenarios.
        /// </summary>
        public ICollectionView EntriesView { get; }

        /// <summary>
        /// Gets the full path where thumbnails are stored.
        /// </summary>
        public string ThumbsDirectory { get; }

        /// <summary>
        /// Gets the seek index base directory.
        /// </summary>
        public string IndexDirectory { get; }

        /// <summary>
        /// Gets the playlist file path.
        /// </summary>
        public string PlaylistFilePath { get; }

        /// <summary>
        /// Gets or sets the playlist search string.
        /// </summary>
        public string PlaylistSearchString
        {
            get => m_PlaylistSearchString;
            set
            {
                if (!SetProperty(ref m_PlaylistSearchString, value))
                    return;

                if (SearchAction == null)
                {
                    SearchAction = DeferredAction.Create(context =>
                    {
                        var futureSearch = PlaylistSearchString ?? string.Empty;
                        var currentSearch = FilterString ?? string.Empty;

                        if (currentSearch == futureSearch) return;
                        if (futureSearch.Length < MinimumSearchLength && currentSearch.Length < MinimumSearchLength) return;

                        EntriesView.Refresh();
                        FilterString = new string(m_PlaylistSearchString.ToCharArray());
                    });
                }

                SearchAction.Defer(SearchActionDelay);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is playlist enabled.
        /// </summary>
        public bool IsPlaylistEnabled
        {
            get => m_IsPlaylistEnabled;
            set => SetProperty(ref m_IsPlaylistEnabled, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in open mode.
        /// </summary>
        public bool IsInOpenMode
        {
            get => m_IsInOpenMode;
            set => SetProperty(ref m_IsInOpenMode, value);
        }

        /// <summary>
        /// Gets or sets the open model URL.
        /// </summary>
        public string OpenMediaSource
        {
            get => m_OpenMediaSource;
            set => SetProperty(ref m_OpenMediaSource, value);
        }

        /// <inheritdoc />
        internal override void OnApplicationLoaded()
        {
            base.OnApplicationLoaded();
            var m = App.ViewModel.MediaElement;

            m.WhenChanged(() => IsPlaylistEnabled = m.IsOpening == false, nameof(m.IsOpening));
            m.MediaOpened += OnMediaOpened;
            m.RenderingVideo += OnRenderingVideo;
        }

        /// <summary>
        /// Called when Media is opened.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnMediaOpened(object sender, EventArgs e)
        {
            HasTakenThumbnail = false;
            var m = App.ViewModel.MediaElement;

            Entries.AddOrUpdateEntry(m.MediaInfo);
            Entries.SaveEntries();
        }

        /// <summary>
        /// Handles the RenderingVideo event of the Media control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenderingVideoEventArgs"/> instance containing the event data.</param>
        private void OnRenderingVideo(object sender, RenderingVideoEventArgs e)
        {
            const double snapshotPosition = 3;

            if (HasTakenThumbnail) return;

            var state = e.EngineState;
            var mediaElement = sender as MediaElement;
            var info = mediaElement?.MediaInfo;

            if (string.IsNullOrWhiteSpace(info?.MediaSource))
                return;

            if (!state.HasMediaEnded && state.Position.TotalSeconds < snapshotPosition &&
                (!state.PlaybackEndTime.HasValue || state.PlaybackEndTime.Value.TotalSeconds > snapshotPosition))
                return;

            HasTakenThumbnail = true;
            Entries.AddOrUpdateEntryThumbnail(info, e.Bitmap);
            Entries.SaveEntries();
        }
    }
}
