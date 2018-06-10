/*// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using PaderbornUniversity.SILab.Hip.Mobile.Ios.CustomRenderers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomFontLabel), typeof(IosCustomFontLabelRenderer))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.CustomRenderers
{
    public class IosCustomFontLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            CustomFontLabel label = elementChangedEventArgs.NewElement as CustomFontLabel;

            if (label?.FontFamilyName != null && label.FontFamilyName != "DEFAULT")
            {
                Control.Font = UIFont.FromName(label.FontFamilyName, Control.Font.PointSize);
            }
        }
    }
}*/