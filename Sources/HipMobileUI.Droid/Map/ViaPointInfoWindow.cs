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

using Android.Content;
using Android.Widget;
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Views;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Object = Java.Lang.Object;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Map {
    class ViaPointInfoWindow : MarkerInfoWindow {

        private string markerId;

        

        public ViaPointInfoWindow (int layoutResId, MapView mapView, Context context) : base (layoutResId, mapView)
        {
            Button infoButton = View.FindViewById<Button> (Resource.Id.bubble_info);

            infoButton.Click += (sender, e) => {
                if (markerId != null)
                {
                    IoCManager.Resolve<INavigationService> ().PushAsync (new ExhibitDetailsViewModel (markerId));
                }
            };
        }

        public override void OnOpen (Object item)
        {
            Marker marker = (Marker) item;
            markerId
                = (
                string
                )
                marker.RelatedObject;

            base.
                OnOpen (item);
        }

    }
}