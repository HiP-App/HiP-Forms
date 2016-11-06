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


using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Text;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity (MainLauncher = true, Theme = "@style/AppTheme")]
    public class AutoCarouselPageActivity : FragmentActivity, ViewPager.IOnPageChangeListener {

        // number of pages in a carousel
        private const int NUM = 3;
        private Button btnSkip, btnNext;

        // needed for the dots diplayed in the bottom of the carousel
        private TextView[] dots;
        private LinearLayout dotsLayout;

        //page adapater; provides concrete page to the ViewPager
        private PagerAdapter pagerAdapter;

        private ISharedPreferences sharedPreferences;

        // pager wigdet, handles animatin and aloow swiping horizontally
        private ViewPager viewPager;

        public void OnPageScrollStateChanged (int state)
        {
        }

        public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected (int position)
        {
            AddButtomDots (position);

            if (position == NUM - 1)
            {
                btnSkip.Visibility = ViewStates.Gone;
                btnNext.Text = GetString (Resource.String.start);
            }
            else
            {
                btnSkip.Visibility = ViewStates.Visible;
                btnNext.Text = GetString (Resource.String.next);
            }
        }

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (this);
            
            if (!sharedPreferences.GetBoolean (Resources.GetString (Resource.String.pref_first_time_launch_key), true))
            {
                launchSplashScreen ();
                Finish ();
            }
            
            // Create the  page fragment here
            SetContentView (Resource.Layout.activity_auto_carousel);
            viewPager = FindViewById<ViewPager> (Resource.Id.view_pager);
            pagerAdapter = new ScreenSlidePageAdapter (SupportFragmentManager);
            viewPager.Adapter = pagerAdapter;

            viewPager.AddOnPageChangeListener (this);

            dotsLayout = FindViewById<LinearLayout> (Resource.Id.layoutDots);
            btnSkip = FindViewById<Button> (Resource.Id.btn_skip);
            btnNext = FindViewById<Button> (Resource.Id.btn_next);

            MakeNotificationBarDisappear();

            AddButtomDots (0);

            btnNext.Click += delegate {
                var current = viewPager.CurrentItem + 1;
                if (current < NUM)
                {
                    viewPager.CurrentItem = current;
                }
                else
                {
                    launchSplashScreen ();
                }
            };

            btnSkip.Click += delegate { launchSplashScreen (); };
        }

        private void launchSplashScreen ()
        {
            var edit = sharedPreferences.Edit ();
            edit.PutBoolean (Resources.GetString (Resource.String.pref_first_time_launch_key), false);
            edit.Commit ();
            StartActivity (typeof (SplashScreenActivity));
            Finish ();
        }

        private void MakeNotificationBarDisappear ()
        {

            // Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                //View decor = Window.DecorView;
                //decor.SystemUiVisibility = StatusBarVisibility.Hidden;
                
               // Window.SetStatusBarColor(Resources.GetColor(Resource.Color.colorPrimaryDark));
            }
            
        }

        public void AddButtomDots (int currentPage)
        {
            dots = new TextView[NUM];

            dotsLayout.RemoveAllViews ();

            for (var i = 0; i < NUM; i++)
            {
                dots [i] = new TextView (this);
                dots [i].Text = Html.FromHtml ("&#8226;").ToString ();
                dots [i].TextSize = 35;
                dots [i].SetTextColor (Color.Black);
                dotsLayout.AddView (dots [i]);
            }

            dots [currentPage].SetTextColor (Color.White);
        }

        public override void OnBackPressed ()
        {
            if (viewPager.CurrentItem == 0)
            {
                base.OnBackPressed ();
            }
            else
            {
                viewPager.CurrentItem = viewPager.CurrentItem - 1;
            }
        }

        private class ScreenSlidePageAdapter : FragmentStatePagerAdapter {

            public ScreenSlidePageAdapter (FragmentManager fm) : base (fm)
            {
            }

            public override int Count {
                get { return NUM; }
            }

            public override Fragment GetItem (int position)
            {
                return AutoCarouselPageFragment.Create (position);
                //return new ScreenSlidePageFragment();
            }

        }

    }
}