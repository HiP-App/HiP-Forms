using System;
using System.Threading;
using Android.Media;
using Android.Util;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HipMobileUI.AudioPlayer;
using Java.IO;
namespace de.upb.hip.mobile.droid.Contracts
{
    class DroidAudioPlayer : IAudioPlayer {

        public DroidAudioPlayer ()
        {
            mediaPlayer = new MediaPlayer ();
            mediaPlayer.SetAudioStreamType(Stream.Music);
        }

        private void UpdateProgress (object state)
        {
            automaticUpdate = true;
            var oldProgress = CurrentProgress;
            CurrentProgress = mediaPlayer.CurrentPosition;
            ProgressChanged?.Invoke (oldProgress, CurrentProgress);
            automaticUpdate = false;
        }

        private bool automaticUpdate;

        private MediaPlayer mediaPlayer;
        private Audio currentAudio;
        private Timer progressUpdateTimer;
        private double currentProgress;

        public bool IsPlaying { get; }

        public double CurrentProgress {
            get { return currentProgress; }
            set {
                currentProgress = value;
                if(!automaticUpdate)mediaPlayer.SeekTo (Convert.ToInt32(value));
            }
        }

        public double MaximumProgress { get; private set; }

        public Audio CurrentAudio {
            get { return currentAudio; }
            set {
                currentAudio = value;
                mediaPlayer.Stop ();
                mediaPlayer.Reset ();
                if (value != null)
                {
                    var path = CopyAudioToTemp (value);
                    mediaPlayer.SetDataSource (path);
                    mediaPlayer.Prepare ();
                    MaximumProgress = mediaPlayer.Duration;
                }
            }
        }

        public event ProgressChangedDelegate ProgressChanged;
        public event IsPlayingDelegate IsPlayingChanged;
        public event AudioCompletedDelegate AudioCompleted;
        public void Play ()
        {
            mediaPlayer.Start ();
            StartProgressThread ();
        }

        public void Pause ()
        {
            mediaPlayer.Pause ();
            StopProgressThread ();
        }

        public void Stop ()
        {
            mediaPlayer.Stop ();
            StopProgressThread ();
        }

        private string CopyAudioToTemp(Audio audio)
        {
            var filepath = "";
            try
            {
                var tempMp3 = File.CreateTempFile("temp", ".mp3", new File(System.IO.Path.GetTempPath()));
                tempMp3.DeleteOnExit();
                var fos = new FileOutputStream(tempMp3);
                fos.Write(audio.Data);
                fos.Close();
                filepath = tempMp3.AbsolutePath;
            }
            catch (IOException ioe)
            {
                Log.Warn("AndroidMediaPlayer", "Could not write audio to temp file, exception message:" + ioe.Message);
            }
            return filepath;
        }

        private void StartProgressThread ()
        {
            progressUpdateTimer = new Timer (UpdateProgress, null, 0, 100);
        }

        private void StopProgressThread ()
        {
            progressUpdateTimer.Dispose ();
        }

    }
}