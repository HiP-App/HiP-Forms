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

using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using UIKit;

namespace PaderbornUniversity.SILab.Hip.Mobile.Ios.ViewControllers
{
    public class OrientationViewController : UIViewController {

        private readonly OrientationController controller;

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)

        {
            if (controller == OrientationController.Sensor)
                return true;
            else if (controller == OrientationController.PortraitConstant && toInterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                return true;
            }
            else if (controller == OrientationController.LandscapeConstant && toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
                     toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                return true;
            }
            return false;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (controller == OrientationController.PortraitConstant)
            {
                return UIInterfaceOrientationMask.Portrait;
            }
            else if (controller == OrientationController.LandscapeConstant)
            {
                return UIInterfaceOrientationMask.Landscape;
            }
            else
            {
                return UIInterfaceOrientationMask.All;
            }
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
        {
            if (controller == OrientationController.PortraitConstant)
            {
                return UIInterfaceOrientation.Portrait;
            }
            else if (controller == OrientationController.LandscapeConstant)
            {
                return UIInterfaceOrientation.LandscapeLeft;
            }
            else
            {
                return UIInterfaceOrientation.Unknown;}
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override bool ShouldAutomaticallyForwardRotationMethods
        {
            get {
                if (controller == OrientationController.Sensor)
                    return true;
                return false;
            }
        }

        public OrientationViewController(OrientationController controller, UIViewController[] childs)
        {
            this.controller = controller;
            foreach (UIViewController uiViewController in childs)
            {
                AddChildViewController(uiViewController);
                View.AddSubview(uiViewController.View);
            }
           
        }
    }
}
