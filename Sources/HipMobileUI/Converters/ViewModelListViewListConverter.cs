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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Converters
{
    public class ViewModelListViewListConverter : IValueConverter
    {

        private readonly IViewCreator navigation = IoCManager.Resolve<IViewCreator>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var viewModels = value as IEnumerable<NavigationViewModel>;
                if (viewModels != null)
                {
                    var views = new ObservableCollection<IViewFor>();
                    foreach (var vm in viewModels)
                    {
                        views.Add(navigation.InstantiateView(vm));
                    }

                    return views;
                }
                throw new Exception("Cannot convert if not List of NavigationPageViewModel!");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}