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

using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Util;
using de.upb.hip.mobile.droid;
using HipMobileUI.Views;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Link), typeof(LinkRenderer))]
namespace de.upb.hip.mobile.droid
{
    class LinkRenderer : LabelRenderer
    {
        private Link formslink;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (elementChangedEventArgs.NewElement != null)
            {
                formslink = (Link) elementChangedEventArgs.NewElement;
                Control.TextFormatted = Html.FromHtml(formslink.Text);
                Control.MovementMethod = LinkMovementMethod.Instance;
                Control.AutoLinkMask = MatchOptions.All;
                Control.Clickable = true;
                Control.LinksClickable = true;
            }
        }
    }
}