// Copyright (C) 2017 History in Paderborn App - Universit�t Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Widget;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Java.IO;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using Plugin.CurrentActivity;
using File = Java.IO.File;
using IOException = Java.IO.IOException;
using Stream = Android.Media.Stream;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { PlayPauseAction, StopAction })]
    internal class DroidAudioPlayer : BroadcastReceiver, IAudioPlayer
    {
        private readonly MediaPlayer mediaPlayer;
        private Audio currentAudio;
        private Timer progressUpdateTimer;

        public DroidAudioPlayer()
        {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.SetAudioStreamType(Stream.Music);
            mediaPlayer.Completion += MediaPlayerOnCompletion;
        }

        public bool IsPlaying => mediaPlayer.IsPlaying;

        public double CurrentProgress => mediaPlayer.CurrentPosition;

        public double MaximumProgress => mediaPlayer.Duration;

        public Audio CurrentAudio
        {
            get { return currentAudio; }
            set
            {
                currentAudio = value;
                DismissNotification();
                mediaPlayer.Stop();
                mediaPlayer.Reset();
                if (value != null)
                {
                    var path = CopyAudioToTemp(value);
                    mediaPlayer.SetDataSource(path);
                    mediaPlayer.Prepare();
                    ProgressChanged?.Invoke(0);
                }
            }
        }

        public string AudioTitle { private get; set; }

        public event ProgressChangedDelegate ProgressChanged;
        public event IsPlayingDelegate IsPlayingChanged;
        public event AudioCompletedDelegate AudioCompleted;

        private static readonly int AudioPlayerNotificationId = 14041993;
        private const string PlayPauseAction = "PaderbornUniversity.SILab.Hip.Mobile.Droid.PlayPause";
        private const string StopAction = "PaderbornUniversity.SILab.Hip.Mobile.Droid.Stop";

        private void ShowNotification(bool setPlayImage)
        {
            var mainActivity = (MainActivity) CrossCurrentActivity.Current.Activity;

            var builder = new NotificationCompat.Builder(mainActivity)
                .SetOngoing(true)
                .SetSmallIcon(Resource.Drawable.ic_headset_white);

            var contentView = new RemoteViews(mainActivity.PackageName, Resource.Layout.audioNotificationView);

            contentView.SetOnClickPendingIntent(Resource.Id.btnPlayPause, GetIntentForAction(PlayPauseAction));
            contentView.SetOnClickPendingIntent(Resource.Id.btnStop, GetIntentForAction(StopAction));

            contentView.SetTextViewText(Resource.Id.textViewTitle, AudioTitle);
            contentView.SetImageViewResource(Resource.Id.btnPlayPause, setPlayImage ? Resource.Drawable.ic_pause : Resource.Drawable.ic_play_arrow);
            contentView.SetProgressBar(Resource.Id.audio_progress_bar, mediaPlayer.Duration, mediaPlayer.CurrentPosition, false);

            builder = builder.SetCustomContentView(contentView);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                builder.SetVisibility((int) NotificationVisibility.Public);
            }

            // Finally, publish the notification
            var notificationManager = (NotificationManager) mainActivity.GetSystemService(Context.NotificationService);
            //notificationManager.Notify(AudioPlayerNotificationId, notificationBuilder.Build ());
            notificationManager.Notify(AudioPlayerNotificationId, builder.Build());
        }

        private PendingIntent GetIntentForAction(string action)
        {
            var mainActivity = (MainActivity) CrossCurrentActivity.Current.Activity;

            Intent intent = new Intent(mainActivity, typeof(DroidAudioPlayer));
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(mainActivity, 0, intent, 0);
        }

        private void DismissNotification()
        {
            var mainActivity = (MainActivity) CrossCurrentActivity.Current.Activity;
            var notificationManager = (NotificationManager) mainActivity.GetSystemService(Context.NotificationService);

            notificationManager.Cancel(AudioPlayerNotificationId);
        }

        public void Play()
        {
            mediaPlayer.Start();
            IsPlayingChanged?.Invoke(true);
            StartUpdateTimer();

            ShowNotification(true);
        }

        public void Pause()
        {
            mediaPlayer.Pause();
            IsPlayingChanged?.Invoke(false);
            StopUpdateTimer();
            ShowNotification(false);
        }

        public void Stop()
        {
            mediaPlayer.Stop();
            StopUpdateTimer();
            IsPlayingChanged?.Invoke(false);
            DismissNotification();
            ProgressChanged?.Invoke(0);

            if (currentAudio != null)
            {
                mediaPlayer.Reset();
                var path = CopyAudioToTemp(currentAudio);
                mediaPlayer.SetDataSource(path);
                mediaPlayer.Prepare();
            }
        }

        public void SeekTo(double progress)
        {
            mediaPlayer.SeekTo(Convert.ToInt32(progress));
        }

        /// <summary>
        /// Called when the media player finishes playing. Stops the update timer, informs listeners and updates the isPlaying state.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event parameters</param>
        private void MediaPlayerOnCompletion(object sender, EventArgs eventArgs)
        {
            StopUpdateTimer();
            AudioCompleted?.Invoke();
            IsPlayingChanged?.Invoke(IsPlaying);
        }

        /// <summary>
        /// Informs all listeners about an updated progress.
        /// </summary>
        /// <param name="state">Ignored.</param>
        private void UpdateProgress(object state)
        {
            ProgressChanged?.Invoke(CurrentProgress);
            ShowNotification(IsPlaying);
        }

        /// <summary>
        /// Copies the audio data to a temporary file, so the media player can play this.
        /// </summary>
        /// <param name="audio">The audio object which data should be copied.</param>
        /// <returns>The string to the file path.</returns>
        private string CopyAudioToTemp(Audio audio)
        {
            var filepath = "";
            try
            {
                string tempFileName = "temp_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                var tempMp3 = File.CreateTempFile(tempFileName, ".mp3", new File(Path.GetTempPath()));
                tempMp3.DeleteOnExit();
                var fos = new FileOutputStream(tempMp3);
                fos.Write(audio.GetDataBlocking());
                fos.Close();
                filepath = tempMp3.AbsolutePath;
            }
            catch (IOException ioe)
            {
                Log.Warn("AndroidMediaPlayer", "Could not write audio to temp file, exception message:" + ioe.Message);
            }
            return filepath;
        }

        /// <summary>
        /// Starts a timer that fires an progress update event at a fixed rate.
        /// </summary>
        private void StartUpdateTimer()
        {
            progressUpdateTimer = new Timer(UpdateProgress, null, 0, 16);
        }

        /// <summary>
        /// Stops the timer sending progress updates.
        /// </summary>
        private void StopUpdateTimer()
        {
            progressUpdateTimer?.Dispose();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var player = IoCManager.Resolve<IAudioPlayer>();

            switch (intent.Action)
            {
                case PlayPauseAction:
                    if (player.IsPlaying)
                    {
                        player.Pause();
                    }
                    else
                    {
                        player.Play();
                    }
                    break;
                case StopAction:
                    player.Stop();
                    break;
            }
        }
    }
}