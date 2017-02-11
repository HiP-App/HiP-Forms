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
using HipMobileUI.Effects;
using HipMobileUI.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Hip")]
[assembly: ExportEffect(typeof(IosSwitchColorEffect), "SwitchColorEffect")]
namespace HipMobileUI.iOS.Effects
{
    class IosSwitchColorEffect : PlatformEffect {

        private UIColor oldColor;

        protected override void OnAttached ()
        {
            //store the old color and set the new one
            UISwitch uiSwitch = (UISwitch) Control;
            SwitchColorEffect effect = (SwitchColorEffect)Element.Effects.FirstOrDefault(e => e is SwitchColorEffect);
            oldColor = uiSwitch.OnTintColor;
            uiSwitch.OnTintColor = effect.Color.ToUIColor ();
        }

        protected override void OnDetached ()
        {
            //restore the old color
            UISwitch uiSwitch = (UISwitch)Control;
            uiSwitch.OnTintColor = oldColor;
        }

    }
}
