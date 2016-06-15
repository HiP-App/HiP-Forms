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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace de.upb.hip.mobile.droid.fragments.exhibitpagefragment
{
    /// <summary>
    /// A fragment for displaying an image with selectable areas.
    /// </summary>
    public class ImagePageExhibitFragment : ExhibitPageFragment
    {

        public static readonly string INSTANCE_STATE_PAGE = "insanceStatePage";

        private ImagePage page;

        private DrawView drawView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View v = inflater.Inflate(Resource.Layout.fragment_exhibitpage_image, container, false);

            if (savedInstanceState != null && savedInstanceState.GetSerializable(INSTANCE_STATE_PAGE) != null)
            {
                page = (ImagePage)savedInstanceState.GetSerializable(INSTANCE_STATE_PAGE);
            }

            drawView = (DrawView)v.FindViewById(Resource.Id.fragment_exhibitpage_image_imageview);
            drawView.SetImageDrawable(this.page.Image.GetDrawable());
            if (page.Areas != null)
            {
                drawView.Rectangles.AddRange(page.Areas);
            }
            else
            {
                //There are no areas to highlight, don't show button
                Button button = (Button)v.FindViewById(Resource.Id.fragment_exhibitpage_image_button);
                button.Visibility = ViewStates.Invisible;
            }
            drawView.OriginalImageDimensions = page.Image.GetDimensions();

            InitListeners(v);

            return v;
        }

        public override void OnSaveInstanceState(Bundle bundle)
        {
            base.OnSaveInstanceState(bundle);
            //bundle.PutSerializable(INSTANCE_STATE_PAGE, page); // TODO handle recreation?
        }

        public override BottomSheetConfig GetBottomSheetConfig()
        {
            SimpleBottomSheetFragment bottomSheetFragment = new SimpleBottomSheetFragment();
            bottomSheetFragment.Title = page.Image.Title;
            bottomSheetFragment.Description = page.Image.Description;
            var bottomSheetConfig = new BottomSheetConfig()
            {
                DisplayBottomSheet = true,
                BottomSheetFragment = bottomSheetFragment
            };
            return bottomSheetConfig;
        }

        public override void SetPage(Page page)
        {
            this.page = page.ImagePage;
        }

        private double[] GetImageScalingFactor()
        {
            int[] originalImageDimensions = page.Image.GetDimensions();
            double widthScalingFactor = ((double)originalImageDimensions[0]) / ((double)drawView.Width);
            double heightScalingFactor = ((double)originalImageDimensions[1]) / ((double)drawView.Height);
            return new[] { widthScalingFactor, heightScalingFactor };
        }

        private void InitListeners(View v)
        {
            drawView.SetOnTouchListener(new CustomOnTouchListener(this));

            Button button = (Button)v.FindViewById(Resource.Id.fragment_exhibitpage_image_button);
            button.Click += (sender, args) =>
            {
                drawView.DrawOnImage = !drawView.DrawOnImage;
                drawView.Invalidate();
            };
        }

        private class CustomOnTouchListener: View.IOnTouchListener
        {
            private ImagePageExhibitFragment parent;

            public CustomOnTouchListener(ImagePageExhibitFragment parent)
            {
                this.parent = parent;
            }
            public void Dispose()
            {
            }

            public IntPtr Handle { get; }
            public bool OnTouch(View v, MotionEvent e)
            {
                if (e.Action != MotionEventActions.Down || parent.page == null)
                {
                    //Only do something when the user actually pressed down and there are actually
                    //areas that the user can press
                    return false;
                }
                int x = (int)(((double) e.GetX()) * parent.GetImageScalingFactor()[0]);
                int y = (int)(((double) e.GetY()) * parent.GetImageScalingFactor()[1]);
                for (int i = 0; i<parent.page.Areas.Count; i++) {
                    Rectangle rect = parent.page.Areas[i];
                    if (x >= rect.Left && x <= rect.Right && y >= rect.Bottom && y <= rect.Top) {
                        //We hit an rectangle, display further information about it
                        new AlertDialog.Builder(Application.Context)
                                .SetTitle(Resource.String.information)
                                .SetMessage(parent.page.Texts[i].Value)
                                .SetPositiveButton(Android.Resource.String.Ok, (sender, args) => { })
                                .SetIcon(Android.Resource.Drawable.IcDialogInfo)
                                .Show();
                        return true;
                    }
}
                return false;
            }
            
        }
    }
}