// /*
//  * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  *
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using HockeyApp;

namespace de.upb.hip.mobile.droid.Activities
{
    // A custom class for changing the layout of the HockeyApp Feedback Activity
    // Adds a back button to the action bar and harmonizes the colors
    [Activity(Label = "Feedback Activity")]
    public class HiPFeedbackActivity : FeedbackActivity
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetBackgroundDrawable (new ColorDrawable(Resources.GetColor (Resource.Color.colorPrimary)));


            var buttonBackgroundColor = Resources.GetColor (Resource.Color.colorPrimary);
            var buttonTextColor = Color.White;

            var buttonAttachment = (Button)FindViewById(Resource.Id.button_attachment);
            buttonAttachment.SetBackgroundColor (buttonBackgroundColor);
            buttonAttachment.SetTextColor (buttonTextColor);

            var buttonSend = (Button)FindViewById(Resource.Id.button_send);
            buttonSend.SetBackgroundColor (buttonBackgroundColor);
            buttonSend.SetTextColor (buttonTextColor);

            var buttonAddResponse = (Button)FindViewById(Resource.Id.button_add_response);
            buttonAddResponse.SetBackgroundColor (buttonBackgroundColor);
            buttonAddResponse.SetTextColor (buttonTextColor);

            var buttonRefresh = (Button)FindViewById(Resource.Id.button_refresh);
            buttonRefresh.SetBackgroundColor (buttonBackgroundColor);
            buttonRefresh.SetTextColor (buttonTextColor);

            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetStatusBarColor(Resources.GetColor (Resource.Color.colorPrimaryDark));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish ();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}