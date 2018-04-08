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

using Android.Content;
using PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ButtonRenderer = Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(ZeroPaddingButton), typeof(ZeroPaddingRenderer))]
namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.CustomRenderers
{
    public class ZeroPaddingRenderer : ButtonRenderer
    {
        public ZeroPaddingRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            Control?.SetPadding(0, 0, 0, 0);
            Control?.SetBackgroundColor(new Color(224, 224, 224));
        }
    }
}