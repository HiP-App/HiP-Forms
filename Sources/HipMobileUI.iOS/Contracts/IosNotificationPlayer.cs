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
using Foundation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer;
using UIKit;
using UserNotifications;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
{
    public class IosNotificationPlayer : INotificationPlayer
    {
        // Use different IDs for different categories of notifications since notifications with the same ID override each other
        private const string ExhibitNearbyNotificationId = "exhibitNearbyNotification";
        private const string DefaultNotificationId = "defaultNotification";

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

            UNUserNotificationCenter.Current.GetNotificationSettings(settings =>
            {
                // Make sure alerts are still allowed
                if (settings.AlertSetting != UNNotificationSetting.Enabled)
                    return;

                var content = new UNMutableNotificationContent
                {
                    Title = title,
                    Body = text,
                    Sound = UNNotificationSound.Default
                };

                if (bmpData != null)
                {
                    // Maybe someone can fix this in the future; the image should be displayed along the notification
                    //UIImage image = ConvertDataToImage(bmpData);
                    //content.Attachments = new UNNotificationAttachment[];
                }

                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0, false);
                var request = UNNotificationRequest.FromIdentifier((string)id, content, trigger);

                UNUserNotificationCenter.Current.AddNotificationRequest(request, error =>
                {
                    if (error != null)
                    {
                        // TODO: handle error if necessary
                    }
                });
            });
        }

        private UIImage ConvertDataToImage(byte[] bmpData)
        {
            return UIImage.LoadFromData(NSData.FromArray(bmpData));
        }
    }
}