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

using Android.Content;
using Android.Net;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using Plugin.CurrentActivity;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    public class DroidNetworkAccessChecker : INetworkAccessChecker
    {
        public NetworkAccessStatus GetNetworkAccessStatus()
        {
            var mainActivity = (MainActivity) CrossCurrentActivity.Current.Activity;
            ConnectivityManager connectivityManager = (ConnectivityManager) mainActivity.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            if (networkInfo == null || !networkInfo.IsConnected)
            {
                return NetworkAccessStatus.NoAccess;
            }

            switch (networkInfo.Type)
            {
                case ConnectivityType.Wifi:
                    return NetworkAccessStatus.WifiAccess;
                case ConnectivityType.Mobile:
                    return NetworkAccessStatus.MobileAccess;
                default:
                    return NetworkAccessStatus.NoAccess;
            }
        }
    }
}