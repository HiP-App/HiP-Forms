using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime.Services
{
    class DesignModeAudioPlayer : IAudioPlayer
    {
        public bool IsPlaying => false;

        public double CurrentProgress => 0;

        public double MaximumProgress => 0;

        public Audio CurrentAudio { get; set; }

        public string AudioTitle { set { } }

        public event ProgressChangedDelegate ProgressChanged;
        public event IsPlayingDelegate IsPlayingChanged;
        public event AudioCompletedDelegate AudioCompleted;

        public void Pause() { }

        public void Play() { }

        public void SeekTo(double progress) { }

        public void Stop() { }
    }
}
