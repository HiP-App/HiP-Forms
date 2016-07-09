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
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.fragments.bottomsheetfragment;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Object = Java.Lang.Object;

namespace de.upb.hip.mobile.droid.fragments.exhibitpagefragment {
    /// <summary>
    ///     A <see cref="ExhibitPageFragment" /> subclass for the <see cref="TimeSliderPage" />.
    /// </summary>
    public class TimeSliderExhibitPageFragment : ExhibitPageFragment {

        public static readonly string INSTANCE_STATE_PAGE = "insanceStatePage";

        private readonly List<PictureData> mPicDataList = new List<PictureData> ();

        private ImageView mFirstImageView;
        private TextView mImageDescription;
        private ImageView mNextImageView;
        private CustomSeekBar mSeekBar;
        private TextView mThumbSlidingText;

        private TimeSliderPage page;

        private View view;

        public override BottomSheetConfig GetBottomSheetConfig ()
        {
            var bottomSheetFragment = new SimpleBottomSheetFragment ();
            bottomSheetFragment.Title = page.Title;
            bottomSheetFragment.Description = page.Text;
            var bottomSheetConfig = new BottomSheetConfig
            {
                DisplayBottomSheet = true,
                BottomSheetFragment = bottomSheetFragment
            };
            return bottomSheetConfig;
        }

        public override void SetPage (Page page)
        {
            this.page = page.TimeSliderPage;
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            view = inflater.Inflate (Resource.Layout.fragment_exhibitpage_timeslider, container, false);


            if (savedInstanceState?.GetString (INSTANCE_STATE_PAGE) != null)
            {
                var pageId = savedInstanceState.GetString (INSTANCE_STATE_PAGE);
                page = PageManager.GetTimesliderPage (pageId);
            }
            SetData ();
            Init ();

            if (page.HideYearNumbers)
            {
                view.FindViewById(Resource.Id.displayImageSliderSeekBarFirstText).Visibility = ViewStates.Invisible;
                view.FindViewById(Resource.Id.displayImageSliderSeekBarEndText).Visibility = ViewStates.Invisible;
            }

            return view;
        }

        public override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);
            outState.PutString (INSTANCE_STATE_PAGE, page.Id);
        }

        /// <summary>
        ///     initializes the activity, calculates and sets the dots on the slider
        /// </summary>
        private void Init ()
        {
            CalcDotPositions (mPicDataList);

            // set the dots
            mSeekBar = (CustomSeekBar) view.FindViewById (Resource.Id.seekBar);
            mSeekBar.DotList = GetListOfDotPositions (mPicDataList);
            mSeekBar.ProgressDrawable = view.Resources.GetDrawable (Resource.Drawable.customseekbar);

            // set the first picture
            mFirstImageView = (ImageView) view.FindViewById (Resource.Id.displayImageSliderFirstImageView);
            mFirstImageView.SetImageDrawable (mPicDataList [0].Drawable);

            // set the next picture
            mNextImageView = (ImageView) view.FindViewById (Resource.Id.displayImageSliderNextImageView);
            mNextImageView.SetImageDrawable (mPicDataList [1].Drawable);
            mFirstImageView.BringToFront ();

            // set start year on the slider
            var seekBarFirstText =
                (TextView) view.FindViewById (Resource.Id.displayImageSliderSeekBarFirstText);
            seekBarFirstText.Text = mPicDataList [0].Year + " " + GetString (Resource.String.after_christ);

            // set end year on the slider
            var seekBarEndText = (TextView) view.FindViewById (Resource.Id.displayImageSliderSeekBarEndText);
            seekBarEndText.Text = mPicDataList [mPicDataList.Count () - 1].Year + " " + GetString (Resource.String.after_christ);

            mThumbSlidingText = (TextView) view.FindViewById (Resource.Id.displayImageSliderThumbSlidingText);

            mImageDescription = (TextView) view.FindViewById (Resource.Id.displayImageSliderDescriptionText);

            AddSeekBarListener ();

            mImageDescription.Text = page.Images [0].Description;
        }

        /// <summary>
        ///     Calculates the positions of the dots on the slider regarding the years of the pictures
        /// </summary>
        /// <param name="list">List of PictureData Elements</param>
        private void CalcDotPositions (List<PictureData> list)
        {
            // set progress for the first picture
            list [0].DotPosition = 0;

            // set progress for other pictures
            var lSize = list.Count ();
            for (var i = 1; i < lSize; i++)
            {
                if (i + 1 < lSize)
                {
                    var progress = 100 * (list [i].Year - list [i - 1].Year) /
                                   (list [lSize - 1].Year - list [0].Year);
                    list [i].DotPosition = progress + list [i - 1].DotPosition;
                }
                else
                {
                    // set progress for last picture
                    list [i].DotPosition = 100;
                }
            }
        }

        /// <summary>
        ///     add a Listener to the Slider to react to changes
        /// </summary>
        private void AddSeekBarListener ()
        {
            mSeekBar.SetOnSeekBarChangeListener (new CustomOnSeekBarChangeListener (this));
        }

        /// <summary>
        ///     Set the mPicDataList Array with data of the database
        /// </summary>
        private void SetData ()
        {
            for (var i = 0; i < page.Images.Count; i++)
            {
                var picture = new PictureData (page.Images [i].GetDrawable (), Convert.ToInt32 (page.Dates [i].Value));
                mPicDataList.Add (picture);
            }
        }

        /// <summary>
        ///     creates a list for setting dots on Slider bar
        /// </summary>
        /// <param name="list">List of PictureData</param>
        /// <returns>list of dot points</returns>
        private List<int> GetListOfDotPositions (List<PictureData> list)
        {
            var mPicDataProgressList = new List<int> ();

            for (var i = 0; i < list.Count (); i++)
            {
                mPicDataProgressList.Add (list [i].DotPosition);
            }
            return mPicDataProgressList;
        }

        /// <summary>
        ///     helper class for PictureData information
        /// </summary>
        private class PictureData {

            internal readonly Drawable Drawable;
            internal readonly int Year;

            internal int DotPosition;

            public PictureData (Drawable drawable, int year)
            {
                Drawable = drawable;
                Year = year;
                DotPosition = 0;
            }

        }

        private class CustomOnSeekBarChangeListener : Object, SeekBar.IOnSeekBarChangeListener {

            private readonly TimeSliderExhibitPageFragment parent;

            private bool forward = true;
            private int nearest;
            private int progressStart;

            public CustomOnSeekBarChangeListener (TimeSliderExhibitPageFragment parent)
            {
                this.parent = parent;
            }

            /// <summary>
            ///     called when the slider is moved
            /// </summary>
            /// <param name="seekBar">slider bar</param>
            /// <param name="progressValue">new progress value</param>
            /// <param name="fromUser">user, who changed it</param>
            public void OnProgressChanged (SeekBar seekBar, int progressValue, bool fromUser)
            {
                int startNode, nextNode;
                var range = parent.mPicDataList [parent.mPicDataList.Count () - 1].Year - parent.mPicDataList [0].Year;

                // decide the direction (forward or backward)
                forward = progressStart <= progressValue;

                // find closest startNode and nextNode, according to the direction
                // (forward or backward)
                var result = GetNodes (progressValue, forward);
                startNode = result [0];
                nextNode = result [1];

                var actProgressAccordingStartNextNode = Math.Abs (progressValue - parent.mPicDataList [startNode].DotPosition);
                var differenceStartNextNode = Math.Abs (parent.mPicDataList [nextNode].DotPosition -
                                                        parent.mPicDataList [startNode].DotPosition);
                var alpha =
                    (float) actProgressAccordingStartNextNode / differenceStartNextNode;

                // set current image
                parent.mFirstImageView.SetImageDrawable (parent.mPicDataList [startNode].Drawable);
                parent.mFirstImageView.Alpha = 1 - alpha;

                // set next image
                parent.mNextImageView.SetImageDrawable (parent.mPicDataList [nextNode].Drawable);
                parent.mNextImageView.Alpha = alpha;

                parent.mFirstImageView.BringToFront ();

                // for showcase image: get the closest node to actual progress
                nearest = FindClosestNode (result, progressValue);

                parent.mImageDescription.Text = parent.page.Images [nearest].Description;

                // set year over the thumb except first and last picture
                if (progressValue != 0 && progressValue != 100)
                {
                    var xPos = (seekBar.Right - seekBar.Left) / seekBar.Max *
                               seekBar.Progress;
                    parent.mThumbSlidingText.SetPadding (xPos, 0, 0, 0);
                    if (!parent.page.HideYearNumbers)
                    {
                        parent.mThumbSlidingText.Text = (int)(parent.mPicDataList[0].Year + range * (float)progressValue / 100.0) + " " +
                                                    parent.GetString(Resource.String.after_christ);
                    }
                }
                else
                {
                    // set empty text for first and last position
                    parent.mThumbSlidingText.Text = "";
                }
            }

            /// <summary>
            ///     Sets the start point of the movement on the slider
            /// </summary>
            /// <param name="seekBar">Slider bar</param>
            public void OnStartTrackingTouch (SeekBar seekBar)
            {
                progressStart = seekBar.Progress;
            }

            /// <summary>
            ///     Set the image if fading is disabled
            /// </summary>
            /// <param name="seekBar">Slider bar</param>
            public void OnStopTrackingTouch (SeekBar seekBar)
            {
                seekBar.Progress = parent.mPicDataList [nearest].DotPosition;
                parent.mFirstImageView.SetImageDrawable (parent.mPicDataList [nearest].Drawable);
            }

            /// <summary>
            ///     returns the two pictures left and right of the current position on the slider
            /// </summary>
            /// <param name="progressStop">endpoint of the movement on the slider</param>
            /// <param name="forward">indicates the direction of the movement</param>
            /// <returns>the two ids of the pictures left and right of the current position</returns>
            private int[] GetNodes (int progressStop, bool forward)
            {
                for (var i = 0; i < parent.mPicDataList.Count (); i++)
                {
                    if (forward)
                    {
                        if ((progressStop >= parent.mPicDataList [i].DotPosition) &&
                            (progressStop <= parent.mPicDataList [i + 1].DotPosition))
                        {
                            return new[] {i, i + 1};
                        }
                    }
                    else
                    {
                        if (i == 0)
                            i = 1;

                        if (progressStop <= parent.mPicDataList [i].DotPosition &&
                            (progressStop >= parent.mPicDataList [i - 1].DotPosition))
                        {
                            return new[] {i, i - 1};
                        }
                    }
                }
                return new[] {0, 0};
            }

            /// <summary>
            ///     find the closest node in the array to progress
            /// </summary>
            /// <param name="array">array array of points on the slider</param>
            /// <param name="progress">progress current progress on the slider</param>
            /// <returns>id of the closest point on slider</returns>
            private int FindClosestNode (int[] array, int progress)
            {
                int min = 0, max = 0, closestNode;

                // calculate left node of progress (min) and right node of progress (max)
                foreach (var anArray in array)
                {
                    if (parent.mPicDataList [anArray].DotPosition < progress)
                    {
                        if (min == 0)
                        {
                            min = anArray;
                        }
                        else if (parent.mPicDataList [anArray].DotPosition > parent.mPicDataList [min].DotPosition)
                        {
                            min = anArray;
                        }
                    }
                    else if (parent.mPicDataList [anArray].DotPosition > progress)
                    {
                        if (max == 0)
                        {
                            max = anArray;
                        }
                        else if (parent.mPicDataList [anArray].DotPosition <
                                 parent.mPicDataList [max].DotPosition)
                        {
                            max = anArray;
                        }
                    }
                    else
                    {
                        return anArray;
                    }
                }

                // calculate which node is nearest to progress (min or max)
                if (Math.Abs (progress - parent.mPicDataList [min].DotPosition) <
                    Math.Abs (progress - parent.mPicDataList [max].DotPosition))
                {
                    closestNode = min;
                }
                else
                {
                    closestNode = max;
                }

                return closestNode;
            }

        }

    }
}