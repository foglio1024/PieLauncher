using Nostrum.WPF;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Media.Control;
// simplify these pls
using PlayBackStatus = Windows.Media.Control.GlobalSystemMediaTransportControlsSessionPlaybackStatus;
using Session = Windows.Media.Control.GlobalSystemMediaTransportControlsSession;
using SessionManager = Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager;

namespace PieLauncher
{
    public class MediaInfoViewModel : ObservableObject
    {
        SessionManager _sessionManager;
        Session? _currentSession;
        readonly DispatcherTimer _timer;
        int _addedSeconds = 0;

        string _artist = "";
        string _title = "";
        string _position = "0:00";
        string _duration = "0:00";
        double _completion;
        PlayBackStatus _playbackStatus;
        bool _isEmpty = true;

        public PlayBackStatus PlaybackStatus
        {
            get => _playbackStatus;
            set
            {
                if (_playbackStatus == value) return;
                _playbackStatus = value;
                N();
            }
        }
        public string Artist
        {
            get => _artist;
            set
            {
                if (_artist == value) return;
                _artist = value;
                N();
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                N();
            }
        }
        public string Position
        {
            get => _position;
            set
            {
                if (_position == value) return;
                _position = value;
                N();
            }
        }
        public string Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                N();
            }
        }
        public double Completion
        {
            get => _completion;
            set
            {
                if (_completion == value) return;
                _completion = value;
                N();
            }
        }
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                if (_isEmpty == value) return;
                _isEmpty = value;
                N();
            }
        }

        public ICommand SkipNextCommand { get; }
        public ICommand SkipPreviousCommand { get; }
        public ICommand PlayPauseCommand { get; }



        public MediaInfoViewModel()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += OnTick;

            SkipNextCommand = new RelayCommand(SkipNext);
            SkipPreviousCommand = new RelayCommand(SkipPrevious);
            PlayPauseCommand = new RelayCommand(PlayPause);

            _sessionManager = SessionManager.RequestAsync().GetResults();
            _sessionManager.CurrentSessionChanged += OnCurrentSessionChanged;

            _currentSession = _sessionManager.GetCurrentSession();

            if (_currentSession == null)
            {
                IsEmpty = true;
                return;
            }
            IsEmpty = _currentSession.SourceAppUserModelId == "Chrome"; // todo: better handling of other media sources
            if (IsEmpty) return;

            _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
            _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
            _currentSession.TimelinePropertiesChanged += OnTimelinePropertiesChanged;

            var playbackInfo = _currentSession.GetPlaybackInfo();
            PlaybackStatus = playbackInfo.PlaybackStatus;

            Task.Run(async () =>
            {
                var mediaProperties = await _currentSession.TryGetMediaPropertiesAsync();
                Artist = mediaProperties.Artist;
                Title = mediaProperties.Title;
            });

            var timelineProperties = _currentSession.GetTimelineProperties();
            Completion = (timelineProperties.Position.TotalSeconds) / timelineProperties.MaxSeekTime.TotalSeconds;
            Position = timelineProperties.Position.ToString("mm\\:ss");
            Duration = timelineProperties.MaxSeekTime.ToString("mm\\:ss");
        }

        async Task PlayPause()
        {
            await _currentSession?.TryTogglePlayPauseAsync();
        }

        async Task SkipNext()
        {
            await _currentSession?.TrySkipNextAsync();
        }

        async Task SkipPrevious()
        {
            await _currentSession?.TrySkipPreviousAsync();
        }

        void OnTick(object? sender, EventArgs e)
        {
            if (PlaybackStatus != PlayBackStatus.Playing)
            {
                _timer.Stop();
                return;
            }
            var timelineProperties = _currentSession?.GetTimelineProperties();
            if (timelineProperties != null)
            {
                var actualPosition = timelineProperties.Position.Add(TimeSpan.FromSeconds(++_addedSeconds));
                Position = actualPosition.ToString("mm\\:ss");
                if (timelineProperties.MaxSeekTime.TotalSeconds == 0) return;
                Completion = (actualPosition.TotalSeconds) / timelineProperties.MaxSeekTime.TotalSeconds;
            }

        }
        void OnCurrentSessionChanged(SessionManager sender, CurrentSessionChangedEventArgs args)
        {
            if (_currentSession != null)
            {
                _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                _currentSession.PlaybackInfoChanged -= OnPlaybackInfoChanged;
                _currentSession.TimelinePropertiesChanged -= OnTimelinePropertiesChanged;
            }

            _currentSession = _sessionManager.GetCurrentSession();

            if (_currentSession == null)
            {
                IsEmpty = true;
                return;
            }

            IsEmpty = _currentSession.SourceAppUserModelId == "Chrome"; // todo: better handling of other media sources
            if (IsEmpty) return;
            _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
            _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
            _currentSession.TimelinePropertiesChanged += OnTimelinePropertiesChanged;

        }
        void OnPlaybackInfoChanged(Session sender, PlaybackInfoChangedEventArgs args)
        {
            var playbackInfo = _currentSession?.GetPlaybackInfo();
            if (playbackInfo != null)
            {
                PlaybackStatus = playbackInfo.PlaybackStatus;
            }
        }
        async void OnMediaPropertiesChanged(Session sender, MediaPropertiesChangedEventArgs args)
        {
            var mediaProperties = await _currentSession?.TryGetMediaPropertiesAsync();
            if (mediaProperties != null)
            {
                Artist = mediaProperties.Artist;
                Title = mediaProperties.Title;
            }
        }
        void OnTimelinePropertiesChanged(Session sender, TimelinePropertiesChangedEventArgs args)
        {
            _addedSeconds = 0;
            _timer.Stop();
            var timelineProperties = _currentSession?.GetTimelineProperties();
            if (timelineProperties != null)
            {
                Completion = (timelineProperties.Position.TotalSeconds) / timelineProperties.MaxSeekTime.TotalSeconds;
                Position = timelineProperties.Position.ToString("mm\\:ss");
                Duration = timelineProperties.MaxSeekTime.ToString("mm\\:ss");

                _timer.Start();
            }
        }


    }
}
