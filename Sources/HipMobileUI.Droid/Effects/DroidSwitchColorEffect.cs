// Copyright (C) 2017 History in Paderborn App - Universit�t Paderborn
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
using Android.Support.V7.Widget;
using de.upb.hip.mobile.droid.Effects;
using HipMobileUI.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ResolutionGroupName("Hip")]
[assembly: ExportEffect(typeof(DroidSwitchColorEffect), "SwitchColorEffect")]
namespace de.upb.hip.mobile.droid.Effects
{
    class DroidSwitchColorEffect : PlatformEffect {

        private Color oldColor;

        protected override void OnAttached ()
        {
            //Store the old color and set the new one
            SwitchCompat switchCompat = (SwitchCompat) Control;
            SwitchColorEffect effect = (SwitchColorEffect)Element.Effects.FirstOrDefault(e => e is SwitchColorEffect);
            oldColor = switchCompat.HighlightColor;
            switchCompat.SetHighlightColor (effect.Color.ToAndroid ());
        }

        protected override void OnDetached ()
        {
            //restore the old color
            SwitchCompat switchCompat = (SwitchCompat)Control;
            switchCompat.SetHighlightColor(oldColor);
        }

    }
}