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
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using HockeyApp;
using HockeyApp.Objects;
using Java.Lang;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Views;
using Object = Java.Lang.Object;

namespace de.upb.hip.mobile.droid.Helpers
{
    // A helper class for hooking the HockeyApp Feedback activity
    // Replaces the default Activity with our custom one
    class HipFeedbackListener : FeedbackManagerListener
    {

        public override bool FeedbackAnswered(FeedbackMessage p0)
        {
            return false;
        }

        public override Class FeedbackActivityClass { get { return Java.Lang.Class.FromType(typeof(HiPFeedbackActivity)); } }

    }
}