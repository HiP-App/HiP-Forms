// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using Android.App;
using Android.OS;
using Android.Widget;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using Microsoft.Practices.Unity;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (Theme = "@style/AppTheme", MainLauncher = true)]
    public class SplashScreenActivity : Activity {

        private const int StartupDelay = 2000;
        private Action action;
        private Button buttonRetry;
        private ProgressBar progressBar;
        private TextView textAction;
        private TextView textWaiting;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView (Resource.Layout.activity_splash_screen);

            textAction = (TextView) FindViewById (Resource.Id.splashScreenActionText);
            textWaiting = (TextView) FindViewById (Resource.Id.splashScreenWaitingText);
            progressBar = (ProgressBar) FindViewById (Resource.Id.splashScreenProgressBar);
            buttonRetry = (Button) FindViewById (Resource.Id.splashScreenRetryButton);

            textAction.SetText (Resource.String.splash_screen_loading);
            textWaiting.SetText (Resource.String.splash_screen_waiting);

            // setup IoCManager
            IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess> ();

            action = StartMainActivity;

            var handler = new Handler ();
            handler.PostDelayed (action, StartupDelay);
        }


        private void StartMainActivity ()
        {
            StartActivity (typeof (MainActivity));
        }

    }
}