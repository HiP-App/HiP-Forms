﻿// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers
{
    public static class AppSharedData
    {
        public static readonly GeoLocation PaderbornMainStation = new GeoLocation(51.71352, 8.74021);

        public static readonly GeoLocation PaderbornCenter = new GeoLocation(51.7189205, 8.7545093);

        public static readonly int MinTimeBwUpdates = 10000; //(10 seconds)
        public static readonly int MinDistanceChangeForUpdates = 10; // 10 meters
        public static readonly double ExhibitRadius = 75; // 75m
        public static readonly double RouteRadius = 50; // 50m
        public static int MapZoomLevel = 15;

        public static readonly string WillSleepMessage = "WillSleep";

        public static readonly string WillWakeUpMessage = "WillWakeUp";

        public static int CurrentAchievementsScore() => 
            DbManager.DataAccess.Achievements().GetAchievements()
                .Where(achievement => achievement.IsUnlocked)
                .Sum(achievement => achievement.Points);
    }
}