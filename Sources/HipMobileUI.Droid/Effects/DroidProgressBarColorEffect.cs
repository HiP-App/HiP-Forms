// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using de.upb.hip.mobile.droid.Effects;
using HipMobileUI.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using ProgressBar = Android.Widget.ProgressBar;

[assembly: ExportEffect(typeof(DroidProgressBarColorEffect), "ProgressBarColorEffect")]
namespace de.upb.hip.mobile.droid.Effects
{
    class DroidProgressBarColorEffect : PlatformEffect
    {

        protected override void OnAttached()
        {
            ProgressBar switchCompat = (ProgressBar)Control;
            ProgressBarColorEffect effect = (ProgressBarColorEffect)Element.Effects.FirstOrDefault(e => e is ProgressBarColorEffect);
            SetColor (switchCompat, effect.Color.ToAndroid());
        }

        protected override void OnDetached()
        {
        }

        private void SetColor (ProgressBar bar, Color color)
        {
            LayerDrawable progressDrawable = (LayerDrawable)bar.ProgressDrawable;
            Drawable primaryColor = progressDrawable.GetDrawable(2);
            primaryColor.SetColorFilter (new LightingColorFilter(0, color));
            progressDrawable.SetDrawableByLayerId(progressDrawable.GetId(2), new ClipDrawable(primaryColor, GravityFlags.Left, ClipDrawableOrientation.Horizontal));
        }

    }
}