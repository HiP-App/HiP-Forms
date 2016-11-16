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
using Android.Graphics.Drawables;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Dialogs {
    [Activity (Label = "TooltipWindow")]
    public class TooltipWindow {

        private readonly View contentView;

        private Context context;
        private readonly LayoutInflater inflater;

        private readonly ISharedPreferences sharedPreferences;
        protected PopupWindow tipWindow;
        


        public TooltipWindow (Context context)
        {
            this.context = context;
            tipWindow = new PopupWindow (context);

            inflater = (LayoutInflater) context.GetSystemService (Context.LayoutInflaterService);
            contentView = inflater.Inflate (Resource.Layout.custom_tooltip, null);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (context);

            var tooltipButton = (Button) contentView.FindViewById (Resource.Id.TooltipButton);

            tooltipButton.Click += delegate {
                var edit = sharedPreferences.Edit ();
                edit.PutBoolean (context.Resources.GetString (Resource.String.pref_tooltip_timeslider_onboarding), false);
                edit.Commit ();
                DismissToolTip ();
            };
        }

        public void ShowToolTip (View anchor)
        {
            tipWindow.Height = ViewGroup.LayoutParams.WrapContent;
            tipWindow.Width = ViewGroup.LayoutParams.WrapContent;
            tipWindow.OutsideTouchable = false;
            tipWindow.Focusable = false;
            tipWindow.SetBackgroundDrawable (new BitmapDrawable ());

            tipWindow.ContentView = contentView;

            tipWindow.AnimationStyle = Resource.Style.animationPopUp;

            var screenPos = new int[2];
            anchor.GetLocationOnScreen (screenPos);
            var anchorRect = new Rect (screenPos [0], screenPos [1], screenPos [0] + anchor.Width, screenPos [1] + anchor.Height);

            contentView.Measure (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            var contentViewheight = contentView.MeasuredHeight;
            var contentViewwidth = contentView.MeasuredWidth;

            var positionX = anchorRect.CenterX () - contentViewwidth / 2;
            var positionY = anchorRect.Bottom - anchorRect.Height () / 2;

            tipWindow.ShowAtLocation (anchor, GravityFlags.NoGravity, positionX, positionY);
        }

        public bool IsTooltipShown ()
        {
            if (tipWindow != null && tipWindow.IsShowing)
                return true;
            return false;
        }

        public void CloseTooltip ()
        {
            DismissToolTip ();
        }

        public void DismissToolTip ()
        {
            if (tipWindow != null && tipWindow.IsShowing)
                tipWindow.Dismiss ();
        }

    }
}