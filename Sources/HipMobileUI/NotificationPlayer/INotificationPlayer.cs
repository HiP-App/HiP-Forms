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

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.NotificationPlayer
{
    public interface INotificationPlayer
    {
        /// <summary>
        /// Displays a notification when in range of an unvisited exhibit
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="text">Text for the notification</param>
        /// <param name="data">Data of the image (not icon) to be displayed</param>
        void DisplayExhibitNearbyNotification(string title, string text, byte[] data = null);

        /// <summary>
        /// Displays a simple notification with a custom title, text and image
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="text">Text for the notification</param>
        /// <param name="id">The unique id for the notification</param>
        /// <param name="data">Data of the image (not icon) to be displayed</param>
        void DisplayDefaultNotification(string title, string text, IComparable id, byte[] data = null);
    }
}