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
using Org.Osmdroid.Bonuspack.Overlays;
using Org.Osmdroid.Views;
using Object = Java.Lang.Object;

namespace de.upb.hip.mobile.droid.Helpers
{
    class ViaPointInfoWindow : MarkerInfoWindow
    {

        private string markerID;

        public ViaPointInfoWindow(int layoutResId, MapView mapView, Context context) : base(layoutResId, mapView)
        {
            Button infoButton = this.View.FindViewById<Button>(Resource.Id.bubble_info);

            infoButton.Click += (sender, e) => {
                var intent = new Intent(context, typeof(ExhibitDetailsActivity));
                var exhibit = ExhibitManager.GetExhibit(markerID);
                if (exhibit.Id != null)
                {
                    intent.PutExtra(ExhibitDetailsActivity.INTENT_EXTRA_EXHIBIT_ID, markerID);
                }
                context.StartActivity(intent);
            };
        }

        public override void OnOpen(Object item)
        {
            Marker marker = (Marker)item;
            markerID = (string)marker.RelatedObject;

            base.OnOpen(item);
        }

    }
}