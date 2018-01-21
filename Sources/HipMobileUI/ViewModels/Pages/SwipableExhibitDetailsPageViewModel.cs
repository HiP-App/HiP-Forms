﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages
{
    public class SwipableExhibitDetailsPageViewModel : NavigationViewModel
    {
        private ObservableCollection<ExhibitDetailsViewModel> pages;

        public ObservableCollection<ExhibitDetailsViewModel> Pages
        {
            get => pages;
            set => SetProperty(ref pages, value);
        }

        public SwipableExhibitDetailsPageViewModel(Exhibit exhibit)
        {
            pages = new ObservableCollection<ExhibitDetailsViewModel>(
                exhibit.Pages.Select(page => new ExhibitDetailsViewModel(exhibit, new List<Page> { page }, "title", additionalInformation: false)));
        }
    }
}