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

using Foundation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer;
using UIKit;
using UserNotifications;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
{
    public class IosNotificationPlayer : INotificationPlayer
    {
        private const string NotificationId = "exhibitnearby";

        public void DisplaySimpleNotification(string title, string text, byte[] bmpData)
        {
            UNUserNotificationCenter.Current.GetNotificationSettings(settings =>
            {
                // Make sure alerts are still allowed
                if (settings.AlertSetting != UNNotificationSetting.Enabled)
                    return;

                var content = new UNMutableNotificationContent
                {
                    Title = title,
                    Subtitle = "",
                    Body = text
                };

                if (bmpData != null)
                {
                    //UIImage image = ConvertDataToImage(bmpData);
                    //content.Attachments = new UNNotificationAttachment[];
                }

                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);
                var request = UNNotificationRequest.FromIdentifier(NotificationId, content, trigger);

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