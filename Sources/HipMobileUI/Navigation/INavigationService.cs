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

using System.Reflection;
using System.Threading.Tasks;
using HipMobileUI.Viewmodels;

namespace HipMobileUI.Navigation {
    public interface INavigationService {

        Task PopAsync(bool animate=true);
        Task PopModalAsync(bool animate= true);
        Task PushAsync(NavigationViewModel viewModel, bool animate= true);
        Task PushModalAsync(NavigationViewModel viewModel, bool animate= true);
        Task PopToRootAsync(bool animate= true);
        Task DisplayAlert (string title, string message, string buttonMessage);
        Task<bool> DisplayAlert(string title, string message, string confirmButtonMessage, string cancelButtonMessage);
        Task<string> DisplayActionSheet (string title, string cancel, string destruction, params string[] buttons);
        void RegisterViewModels (Assembly asm);

    }
}