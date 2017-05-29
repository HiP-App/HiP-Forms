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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels;
using Xamarin.Forms;
using MainPage = PaderbornUniversity.SILab.Hip.Mobile.UI.Pages.MainPage;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation
{
    public class NavigationService : INavigationService, IViewCreator {

        #region Singleton

        private static NavigationService instance;

        public static NavigationService Instance {
            get {
                if (instance == null)
                {
                    instance= new NavigationService ();
                }
                return instance;
            }
        }
        #endregion

        readonly Dictionary<Type, Type> viewModelViewDictionary = new Dictionary<Type, Type>();

        INavigation FormsNavigation
        {
            get
            {
                var tabController = Application.Current.MainPage as TabbedPage;
                var masterController = Application.Current.MainPage as MasterDetailPage;

                // First check to see if we're on a tabbed page, then master detail, finally go to overall fallback
                return tabController?.CurrentPage?.Navigation ??
                                     (masterController?.Detail as TabbedPage)?.CurrentPage?.Navigation ?? // special consideration for a tabbed page inside master/detail
                                     masterController?.Detail?.Navigation ??
                                     Application.Current.MainPage.Navigation;
            }
        }

        private Page FormsPage => Application.Current.MainPage;

        private NavigationPage NavigationPage {
            get {
                var mainPage = FormsPage as MainPage;
                return mainPage?.Navigationpage;
            }
        }

        public async Task PopAsync (bool animate=false)
        {
            await FormsNavigation.PopAsync(animate);
        }

        public async Task PopModalAsync (bool animate=false)
        {
            await FormsNavigation.PopModalAsync(animate);

            var currentPageBindingContext = (NavigationViewModel)NavigationPage?.CurrentPage.BindingContext;
            currentPageBindingContext?.OnRevealed ();
        }

        public async Task PushAsync (NavigationViewModel viewModel, bool animate=false)
        {
            try
            {
                var oldViewModel = (NavigationViewModel) NavigationPage?.CurrentPage.BindingContext;
                var view = InstantiateView (viewModel);

                await FormsNavigation.PushAsync ((Page) view, animate);
                oldViewModel?.OnHidden ();
            }
            catch
            {
                // ignored as non critical exceptions don't stop the app from working
            }
        }

        public async Task PushModalAsync (NavigationViewModel viewModel, bool animate=false)
        {
            try
            {
                var oldViewModel = (NavigationViewModel)NavigationPage?.CurrentPage.BindingContext;
                var view = InstantiateView (viewModel);

                // Most likely we're going to want to put this into a navigation page so we can have a title bar on it
                var nv = new NavigationPage ((Page) view);

                await FormsNavigation.PushModalAsync (nv, animate);
                oldViewModel?.OnHidden();
            }
            catch
            {
                // ignored as non critical exceptions don't stop the app from working
            }
        }

        public void RemovePage (NavigationViewModel viewModel)
        {
            var page = FormsNavigation.NavigationStack.FirstOrDefault (x => x.BindingContext == viewModel);
            FormsNavigation.RemovePage (page);
        }

        public void InsertPageBefore (NavigationViewModel viewModel, NavigationViewModel before)
        {
            var pageBefore = FormsNavigation.NavigationStack.FirstOrDefault(x => x.BindingContext == before);
            var view = InstantiateView(viewModel);

            FormsNavigation.InsertPageBefore ((Page)view, pageBefore);
        }

        public async Task PopToRootAsync (bool animate=false)
        {
            await FormsNavigation.PopToRootAsync(animate);
        }

        public async Task DisplayAlert (string title, string message, string buttonMessage)
        {
             await FormsPage.DisplayAlert (title, message, buttonMessage);
        }

        public async Task<bool> DisplayAlert (string title, string message, string confirmButtonMessage, string cancelButtonMessage)
        {
            return await FormsPage.DisplayAlert (title, message, confirmButtonMessage, cancelButtonMessage);
        }

        public async Task<string> DisplayActionSheet (string title, string cancel, string destruction, params string[] buttons)
        {
            return await FormsPage.DisplayActionSheet (title, cancel, destruction, buttons);
        }

        public void RegisterViewModels (Assembly asm)
        {
            // Loop through everything in the assembley that implements IViewFor<T>
            foreach (var type in asm.DefinedTypes.Where(dt => !dt.IsAbstract &&
                dt.ImplementedInterfaces.Any(ii => ii == typeof(IViewFor))))
            {

                // Get the IViewFor<T> portion of the type that implements it
                var viewForType = type.ImplementedInterfaces.FirstOrDefault(
                    ii => ii.IsConstructedGenericType &&
                    ii.GetGenericTypeDefinition() == typeof(IViewFor<>));

                // Register it, using the T as the key and the view as the value
                Register(viewForType.GenericTypeArguments[0], type.AsType());
            }
        }

        public void StartNewNavigationStack (NavigationViewModel newRoot)
        {
            // instatiate the view for the viewmodel and set it as the new root
            Page rootView = (Page)InstantiateView (newRoot);
            Application.Current.MainPage = rootView;
        }

        public void Register(Type viewModelType, Type viewType)
        {
            if (!viewModelViewDictionary.ContainsKey (viewModelType))
            {
                viewModelViewDictionary.Add (viewModelType, viewType);
            }
        }

        public IViewFor InstantiateView(NavigationViewModel viewModel)
        {
            // Figure out what type the view model is
            var viewModelType = viewModel.GetType();

            // look up what type of view it corresponds to
            var viewType = viewModelViewDictionary[viewModelType];

            // instantiate it
            var view = (IViewFor)Activator.CreateInstance(viewType);

            ((BindableObject)view).BindingContext= viewModel;

            return view;
        }

    }
}
