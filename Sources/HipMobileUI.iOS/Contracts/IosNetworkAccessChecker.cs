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

using System.Net;
using SystemConfiguration;
using CoreFoundation;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Contracts
{
    public class IosNetworkAccessChecker : INetworkAccessChecker
    {
        public NetworkAccessStatus GetNetworkAccessStatus()
        {
            NetworkReachabilityFlags flags;
            bool defaultNetworkAvailable = IsNetworkAvailable(out flags);

            if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
                return NetworkAccessStatus.NoAccess;

            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                return NetworkAccessStatus.MobileAccess;

            if (flags == 0)
                return NetworkAccessStatus.NoAccess;

            return NetworkAccessStatus.WifiAccess;
        }

        private NetworkReachability defaultRouteReachability;

        private bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            if (defaultRouteReachability == null)
            {
                defaultRouteReachability = new NetworkReachability("google.com");
                defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
            }
            return defaultRouteReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
        }

        private bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
                || (flags & NetworkReachabilityFlags.IsWWAN) != 0;

            return isReachable && noConnectionRequired;
        }
    }
}