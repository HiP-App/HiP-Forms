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
using PaderbornUniversity.SILab.Hip.Mobile.Ios.Effects;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(IosProgressBarColorEffect), "ProgressBarColorEffect")]
namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.Effects
{
    class IosProgressBarColorEffect : PlatformEffect
    {

        private UIColor oldColor;

        protected override void OnAttached ()
        {
            UIProgressView progressBar = (UIProgressView)Control;
            ProgressBarColorEffect effect = (ProgressBarColorEffect)Element.Effects.FirstOrDefault(e => e is ProgressBarColorEffect);
            oldColor = progressBar.ProgressTintColor;
            if (effect != null)
                progressBar.ProgressTintColor = effect.Color.ToUIColor ();
        }

        protected override void OnDetached ()
        {
            //restore the old color
            UIProgressView uiSwitch = (UIProgressView)Control;
            uiSwitch.ProgressTintColor = oldColor;
        }

    }
}
