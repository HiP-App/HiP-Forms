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

using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Util;
using Java.Lang;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    public class AndroidAppCloser : IAppCloser
    {
        public void RestartOrClose()
        {
            var packageManager = Application.Context.PackageManager;
            var intent = packageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
            var componentName = intent.Component;
            var mainIntent = Intent.MakeRestartActivityTask(componentName);
            mainIntent.AddFlags(ActivityFlags.ClearTop);

            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, mainIntent,
                                                          PendingIntentFlags.CancelCurrent);
            var mgr = (AlarmManager) Application.Context.GetSystemService(Context.AlarmService);
            mgr.Set(AlarmType.Rtc, JavaSystem.CurrentTimeMillis() + 1000, pendingIntent);

            Log.Debug("AndroidAppCloser", "Closing!");
            Runtime.GetRuntime().Exit(0);
        }
    }
}