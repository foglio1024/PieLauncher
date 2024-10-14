using System.Windows.Threading;
using Windows.Media.Control;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
// simplify these pls
using PlayBackStatus = Windows.Media.Control.GlobalSystemMediaTransportControlsSessionPlaybackStatus;
using Session = Windows.Media.Control.GlobalSystemMediaTransportControlsSession;
using SessionManager = Windows.Media.Control.GlobalSystemMediaTransportControlsSessionManager;

namespace PieLauncher.Core.ViewModels;

public partial class MediaInfoViewModel : ObservableObject
{
    private SessionManager? _sessionManager;
    private SessionManager SessionManager
    {
        get
        {
            if (_sessionManager == null)
            {
                _sessionManager = SessionManager.RequestAsync().GetResults();
                _sessionManager.CurrentSessionChanged += OnCurrentSessionChanged;
                _currentSession = _sessionManager.GetSessions().AsEnumerable<Session>().FirstOrDefault(x =>
                    x.SourceAppUserModelId == "Spotify.exe");

                if (_currentSession == null)
                {
                    IsEmpty = true;
                    return _sessionManager;
                }
                IsEmpty = _currentSession.SourceAppUserModelId != "Spotify.exe"; // todo: better handling of other media sources
                if (IsEmpty) return _sessionManager;

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

            return _sessionManager;
        }
    }

    private Session? _currentSession;
    private readonly DispatcherTimer _timer;
    private int _addedSeconds;

    [ObservableProperty]
    private string _artist = "";

    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _position = "0:00";

    [ObservableProperty]
    private string _duration = "0:00";

    [ObservableProperty]
    private double _completion;

    [ObservableProperty]
    private PlayBackStatus _playbackStatus = PlayBackStatus.Playing;

    [ObservableProperty]
    private bool _isEmpty = true;

    public MediaInfoViewModel()
    {
        _timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTick;
    }

    [RelayCommand]
    private async Task PlayPause()
    {
        await _currentSession?.TryTogglePlayPauseAsync();
    }

    [RelayCommand]
    private async Task SkipNext()
    {
        await _currentSession?.TrySkipNextAsync();
    }

    [RelayCommand]
    private async Task SkipPrevious()
    {
        await _currentSession?.TrySkipPreviousAsync();
    }

    private void OnTick(object? sender, EventArgs e)
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

    private void OnCurrentSessionChanged(SessionManager sender, CurrentSessionChangedEventArgs args)
    {
        if (_currentSession != null)
        {
            _currentSession.MediaPropertiesChanged -= OnMediaPropertiesChanged;
            _currentSession.PlaybackInfoChanged -= OnPlaybackInfoChanged;
            _currentSession.TimelinePropertiesChanged -= OnTimelinePropertiesChanged;
        }

        _currentSession = SessionManager.GetSessions().AsEnumerable<Session>().FirstOrDefault(x => x.SourceAppUserModelId == "Spotify.exe");

        if (_currentSession == null)
        {
            IsEmpty = true;
            return;
        }

        IsEmpty = _currentSession.SourceAppUserModelId != "Spotify.exe"; // todo: better handling of other media sources
        if (IsEmpty) return;
        _currentSession.MediaPropertiesChanged += OnMediaPropertiesChanged;
        _currentSession.PlaybackInfoChanged += OnPlaybackInfoChanged;
        _currentSession.TimelinePropertiesChanged += OnTimelinePropertiesChanged;
    }

    private void OnPlaybackInfoChanged(Session sender, PlaybackInfoChangedEventArgs args)
    {
        var playbackInfo = _currentSession?.GetPlaybackInfo();
        if (playbackInfo != null)
        {
            PlaybackStatus = playbackInfo.PlaybackStatus;
        }
    }

    private async void OnMediaPropertiesChanged(Session sender, MediaPropertiesChangedEventArgs args)
    {
        var mediaProperties = await _currentSession?.TryGetMediaPropertiesAsync();
        if (mediaProperties != null)
        {
            Artist = mediaProperties.Artist;
            Title = mediaProperties.Title;
        }
    }

    private void OnTimelinePropertiesChanged(Session sender, TimelinePropertiesChangedEventArgs args)
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