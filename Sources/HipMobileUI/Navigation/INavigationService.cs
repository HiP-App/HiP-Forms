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
using HipMobileUI.ViewModels;

namespace HipMobileUI.Navigation {
    public interface INavigationService {

        /// <summary>
        /// Asynchronously removes the most recent Page from the navigation stack, with optional animation.
        /// </summary>
        /// <param name="animate">Anmiate the removal.</param>
        /// <returns>The task.</returns>
        Task PopAsync(bool animate=true);

        /// <summary>
        /// Asynchronously dismisses the most recent modally presented Page, with optional animation.
        /// </summary>
        /// <param name="animate">Anmiate the removal.</param>
        /// <returns>The task.</returns>
        Task PopModalAsync(bool animate= true);

        /// <summary>
        /// Asynchronously adds a viewmodel to the top of the navigation stack, with optional animation.
        /// </summary>
        /// <param name="viewModel">The viewmodel to be added.</param>
        /// <param name="animate">Animate the push.</param>
        /// <returns>The task.</returns>
        Task PushAsync(NavigationViewModel viewModel, bool animate= true);

        /// <summary>
        /// Presents a viewmodel modally, with optional animation.
        /// </summary>
        /// <param name="viewModel">The viewmodel to be presented.</param>
        /// <param name="animate">Animate the push.</param>
        /// <returns>The task.</returns>
        Task PushModalAsync(NavigationViewModel viewModel, bool animate= true);

        /// <summary>
        /// Pops all but the root viewmodel off the navigation stack, with optional animation.
        /// </summary>
        /// <param name="animate">Animate the pop.</param>
        /// <returns>The task.</returns>
        Task PopToRootAsync(bool animate= true);

        /// <summary>
        /// Presents an alert dialog to the application user with a single cancel button.
        /// </summary>
        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="buttonMessage">Text to be displayed on the 'Cancel' button.</param>
        /// <returns>The task.</returns>
        Task DisplayAlert (string title, string message, string buttonMessage);

        /// <summary>
        /// Presents an alert dialog to the application user with an accept and a cancel button.
        /// </summary>
        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="confirmButtonMessage">Text to be displayed on the 'Accept' button.</param>
        /// <param name="cancelButtonMessage">Text to be displayed on the 'Cancel' button.</param>
        /// <returns>The task.</returns>
        Task<bool> DisplayAlert(string title, string message, string confirmButtonMessage, string cancelButtonMessage);

        /// <summary>
        /// Displays a native platform action sheet, allowing the application user to choose from several buttons.
        /// </summary>
        /// <param name="title">Title of the displayed action sheet. Must not be null.</param>
        /// <param name="cancel">Text to be displayed in the 'Cancel' button. Can be null to hide the cancel action.</param>
        /// <param name="destruction">Text to be displayed in the 'Destruct' button. Can be null to hide the destructive option.</param>
        /// <param name="buttons">Text labels for additional buttons. Must not be null.</param>
        /// <returns>The task.</returns>
        Task<string> DisplayActionSheet (string title, string cancel, string destruction, params string[] buttons);

        /// <summary>
        /// Registers all viewmodels in the assembly to this navigation service.
        /// </summary>
        /// <param name="asm">The assembly, to be parsed.</param>
        void RegisterViewModels (Assembly asm);

        /// <summary>
        /// Resets the current navigation stack and pushes the parameter as the new root.
        /// </summary>
        /// <param name="newRoot">The viewmodel which view is used as the new root.</param>
        void StartNewNavigationStack (NavigationViewModel newRoot);

    }
}