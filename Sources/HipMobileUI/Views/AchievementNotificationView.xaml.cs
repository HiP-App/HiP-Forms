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

using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    public partial class AchievementNotificationView : IViewFor<AchievementNotificationView>
    {
        private DeviceOrientation deviceOrientation;
        public AchievementNotificationView()
        {
            InitializeComponent();
            deviceOrientation = DeviceOrientation.Undefined;
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width <= height)
            {
                if (deviceOrientation != DeviceOrientation.Portrait)
                {
                    // portrait mode
                    AbsoluteLayout.SetLayoutBounds(AchievementNotification, new Rectangle(0.5,0,1,0.2));

                    deviceOrientation = DeviceOrientation.Portrait;
                }
            }
            else if (width > height)
            {
                if (deviceOrientation != DeviceOrientation.Landscape)
                {
                    // landscape mode
                    AbsoluteLayout.SetLayoutBounds(AchievementNotification, new Rectangle(0.5,0,1,0.5));
                    
                    deviceOrientation = DeviceOrientation.Landscape;
                }
            }
        }
    }
}