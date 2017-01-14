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

using System.Linq;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.ViewModels.Pages;
using Xamarin.Forms;

namespace HipMobileUI.ViewModels.Views {
    public class DummyViewModel : NavigationViewModel{

        public DummyViewModel ()
        {
            Color= Color.Blue;
            Title = "Dummy";
            TestCommand = new Command (() => {
                                           var exhibits = ExhibitManager.GetExhibits ();
                                           var ex = exhibits.FirstOrDefault();
                                           Navigation.PushAsync (new ExhibitDetailsViewModel (ex.Id));
                                       });
        }

        private Color color;
        private ICommand testCommand;

        public Color Color {
            get { return color; }
            set { SetProperty (ref color, value); }
        }

        public ICommand TestCommand {
            get { return testCommand; }
            set { SetProperty (ref testCommand, value); }
        }

    }
}