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

using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Util;
using de.upb.hip.mobile.droid.CustomRenderers;
using HipMobileUI.Controls;
using HipMobileUI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomFontLabel), typeof(AndroidCustomFontLabelRenderer))]
namespace de.upb.hip.mobile.droid.CustomRenderers
{
    public class AndroidCustomFontLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            CustomFontLabel label = elementChangedEventArgs.NewElement as CustomFontLabel;

            if (label?.FontFamilyName != null)
            {
                Control.Typeface = Typeface.CreateFromAsset (Context.Assets, $"fonts/{label.FontFamilyName}.ttf");
            }
        }
    }
}