// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using Android.Content;
using Android.OS;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FloatingActionButton), typeof(FloatingActionButtonAndroidRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers
{
    public class FloatingActionButtonAndroidRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<FloatingActionButton,
        Android.Support.Design.Widget.FloatingActionButton>
    {
        private Android.Support.Design.Widget.FloatingActionButton fab;
        private FloatingActionButton formsButton;

        public FloatingActionButtonAndroidRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                fab = new Android.Support.Design.Widget.FloatingActionButton(Context);
                fab.Click += FabOnClick;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    fab.Elevation = 2;

                SetNativeControl(fab);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                e.OldElement.NormalColorChanged -= SetNormalColor;
                e.OldElement.RippleColorChanged -= SetRippleColor;
                e.OldElement.IconChanged -= SetIcon;
            }
            if (e.NewElement != null)
            {
                // set init values
                formsButton = e.NewElement;
                SetIcon(formsButton.Icon);
                SetNormalColor(formsButton.NormalColor);
                SetRippleColor(formsButton.RippleColor);

                // Subscribe
                formsButton.NormalColorChanged += SetNormalColor;
                formsButton.RippleColorChanged += SetRippleColor;
                formsButton.IconChanged += SetIcon;
            }
        }

        private void SetRippleColor(Color newColor)
        {
            fab.RippleColor = newColor.ToAndroid();
        }

        private void SetNormalColor(Color newColor)
        {
            fab.SetBackgroundColor(newColor.ToAndroid());
        }

        private void SetIcon(string newIcon)
        {
            int resourceId = Resources.GetIdentifier(newIcon, "drawable", Context.PackageName);
            fab.SetImageResource(resourceId);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            if (formsButton.Command.CanExecute(null))
            {
                formsButton.Command.Execute(this);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                fab.Click -= FabOnClick;
                formsButton.NormalColorChanged -= SetNormalColor;
                formsButton.RippleColorChanged -= SetRippleColor;
                formsButton.IconChanged -= SetIcon;
            }

            base.Dispose(disposing);
        }
    }
}