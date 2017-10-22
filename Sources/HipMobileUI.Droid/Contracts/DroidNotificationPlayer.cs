// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer;
using Plugin.CurrentActivity;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    public class DroidNotificationPlayer : INotificationPlayer
    {
        // Use different IDs for different categories of notifications since notifications with the same ID override each other
        private const int ExhibitNearbyNotificationId = 30052010;
        private const int DefaultNotificationId = 06031993;

        private const int InfoIcon = Resource.Drawable.info;

        public void DisplayExhibitNearbyNotification(string title, string text, byte[] data = null)
        {
            DisplayDefaultNotification(title, text, ExhibitNearbyNotificationId, data);
        }

        /// <summary>
        /// Display a simple notification with an optional large image
        /// </summary>
        /// <param name="title">Heading for the notification</param>
        /// <param name="text">The message below the title</param>
        /// <param name="id">Unique id for the notification</param>
        /// <param name="bmpData">Data for the large image</param>
        public void DisplayDefaultNotification(string title, string text, IComparable id = null, byte[] bmpData = null)
        {
            if (id == null)
                id = DefaultNotificationId;

            var mainActivity = (MainActivity)CrossCurrentActivity.Current.Activity;

            var builder = new NotificationCompat.Builder(mainActivity)
                .SetContentIntent(GenerateReturnToAppIntent(mainActivity))
                .SetContentTitle(title)
                .SetContentText(text)
                .SetSmallIcon(InfoIcon)
                .SetAutoCancel(true)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));
            if (bmpData != null)
                builder.SetLargeIcon(BitmapFactory.DecodeByteArray(bmpData, 0, bmpData.Length));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                builder.SetVisibility((int)NotificationVisibility.Public);
            }

            var notificationManager = (NotificationManager)mainActivity.GetSystemService(Context.NotificationService);
            notificationManager.Notify((int)id, builder.Build());
        }

        private PendingIntent GenerateReturnToAppIntent(Context mainActivity)
        {
            var intent = new Intent(mainActivity, typeof(MainActivity));
            return PendingIntent.GetActivity(mainActivity, 0, intent, PendingIntentFlags.OneShot);
        }
    }
}