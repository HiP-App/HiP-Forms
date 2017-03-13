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

using System;
using System.Globalization;
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels;
using Xamarin.Forms;

namespace HipMobileUI.Converters {
    public class ViewModelViewConverter : IValueConverter {

        private readonly IViewCreator navigation = IoCManager.Resolve<IViewCreator> ();

        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is NavigationViewModel)
                {
                    var vm = (NavigationViewModel) value;
                    return navigation.InstantiateView (vm);
                }
                throw new Exception ("Cannot convert non NavigationPageViewModel!");
            }
            return null;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is View)
                {
                    var view = (View) value;
                    return view;
                }
                throw new Exception ("Cannot convert non View!");
            }
            return null;
        }

    }
}