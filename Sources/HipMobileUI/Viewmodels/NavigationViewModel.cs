// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.Navigation;
using MvvmHelpers;

namespace HipMobileUI.ViewModels
{
    public abstract class NavigationViewModel : BaseViewModel {

        protected static INavigationService Navigation = IoCManager.Resolve<INavigationService> ();

        /// <summary>
        /// Method called when the view disappears. Note that this method is not called automatically for every view.
        /// </summary>
        public virtual void OnDisappearing ()
        {
        }

    }
}
