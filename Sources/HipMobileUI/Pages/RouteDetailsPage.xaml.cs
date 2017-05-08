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

using System;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages
{
    public partial class RouteDetailsView : IViewFor<RouteDetailsPageViewModel>
    {

        private double width1 = 0;
        private double height1 = 0;

        public RouteDetailsView()
        {
            InitializeComponent();

        }


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width1 != width || height1 != height)
            {
                width1 = width;
                height1 = height;
                if (width > height)
                {
                    OuterLayout.Orientation = StackOrientation.Horizontal;
                }
                else
                {
                    OuterLayout.Orientation = StackOrientation.Vertical;
                }


            }




        }


    }
}

