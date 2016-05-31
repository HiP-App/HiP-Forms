using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Activities
{
    [Activity(Theme = "@style/AppTheme",Label = "SplashScreenActivity",MainLauncher = true, Icon = "@drawable/icon")]
    public class SplashScreenActivity : Activity
    {

        private const int MStartupDelay = 2000;
        private TextView mTextAction;
        private TextView mTextWaiting;
        private ProgressBar mProgressBar;
        private Button mButtonRetry;
        private Action action;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_splash_screen);

            mTextAction = (TextView)FindViewById(Resource.Id.splashScreenActionText);
            mTextWaiting = (TextView)FindViewById(Resource.Id.splashScreenWaitingText);
            mProgressBar = (ProgressBar)FindViewById(Resource.Id.splashScreenProgressBar);
            mButtonRetry = (Button)FindViewById(Resource.Id.splashScreenRetryButton);

            mTextAction.SetText(Resource.String.splash_screen_loading);
            mTextWaiting.SetText(Resource.String.splash_screen_waiting);

            action = new Action(startMainActivity);

            Handler handler = new Handler();
            handler.PostDelayed (action, MStartupDelay);
        }


        private void startMainActivity ()
        {
            StartActivity (typeof (MainActivity));
        }

    }
}
