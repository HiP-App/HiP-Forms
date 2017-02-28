// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System;

using Xamarin.Forms.Maps;

namespace HipMobileUI.Helpers
{
    public static class AppSharedData {

        public static readonly Position PaderbornMainStation = new Position (51.71352, 8.74021);

        public static readonly Position PaderbornCenter = new Position (51.7189205, 8.7575093);

        public static readonly int MinTimeBwUpdates = 2000; //4000 milliseconds (4seconds)
        public static readonly int MinDistanceChangeForUpdates = 10; // 10 metres
        public static readonly double ExhibitRadius = 0.02; // 20m
        public static int MapZoomLevel = 14;
    }
}
