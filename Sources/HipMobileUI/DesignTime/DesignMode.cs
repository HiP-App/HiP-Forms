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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Appearance;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime.Data;
using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime.Services;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime
{
    /// <summary>
    /// Supports the design time experience around the Xamarin.Forms Previewer.
    /// </summary>
    public static class DesignMode
    {
        // view models to use during XAML preview
        private static readonly Dictionary<Type, object> _viewModels = new Dictionary<Type, object>();

        /// <summary>
        /// Indicates whether the app is executed in design mode (i.e. by Xamarin.Forms Previewer).
        /// </summary>
        /// <remarks>
        /// This is set to true on app start in AppDelegate (iOS) and MainActivity (Android).
        /// The Xamarin Forms Previewer does not instantiate those classes, so this flag remains true.
        /// </remarks>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        /// To be called in <see cref="Xamarin.Forms.Page"/>-constructors.
        /// Assigns a sample view model to the view's BindingContext, if the app is executed in design mode.
        /// If (1) no sample view model is available, (2) the view does not implement <see cref="IViewFor{T}"/>,
        /// or (3) the app is running for real (on a device or in an emulator), this method has no effect.
        /// </summary>
        /// <remarks>
        /// This method should be called in the constructor of each page, right after the
        /// "InitializeComponent()"-call.
        /// </remarks>
        /// <param name="view">View to set up for previewing</param>
        public static void Initialize<TView>(TView view) where TView : BindableObject, IViewFor
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var viewForType = view.GetType().GetTypeInfo().ImplementedInterfaces.FirstOrDefault(ii =>
                ii.IsConstructedGenericType &&
                ii.GetGenericTypeDefinition() == typeof(IViewFor<>));

            if (viewForType != null)
            {
                var viewModelType = viewForType.GenericTypeArguments[0];
                view.BindingContext = _viewModels.TryGetValue(viewModelType, out var vm) ? vm : null;
            }
        }

        /// <summary>
        /// To be called in the <see cref="App"/>-constructor. If the app is running in design mode,
        /// this registers sample view models and OS-independent (mock-)services that are required to
        /// preview XAML pages.
        /// </summary>
        /// <remarks>
        /// This method should be called in the <see cref="App"/> constructor, right before
        /// <see cref="App.InitializeComponent"/>.
        /// </remarks>
        public static void Initialize()
        {
            if (!IsEnabled)
                return;

            ConfigureServices();
            ConfigureViewModels();
        }

        private static void ConfigureServices()
        {
            NavigationService.Instance.RegisterViewModels(typeof(MainPage).GetTypeInfo().Assembly);
            IoCManager.RegisterInstance(typeof(INavigationService), NavigationService.Instance);
            IoCManager.RegisterInstance(typeof(IViewCreator), NavigationService.Instance);
            IoCManager.RegisterType<IStatusBarController, DesignTimeStatusBarController>();
            IoCManager.RegisterType<IDataAccess, DesignModeDataAccess>();
            IoCManager.RegisterType<IThemeManager, DesignModeThemeManager>();
        }

        private static void ConfigureViewModels()
        {
            Add(new ExhibitRouteDownloadPageViewModel(
                DesignModeDownloadable.SampleInstance,
                new DesignModeDownloadableListItemViewModel()));

            Add(new LoadingPageViewModel { Title = "Sample Title", Text = "Sample Text" });

            Add(new AchievementsDetailsExhibitViewModel(new ExhibitsVisitedAchievement
            {
                Title = "Lorem ipsum!",
                Description = "You unlocked stuff!",
                Count = 3
            }));

            Add(new AppetizerPageViewModel(new Exhibit
            {
                Name = "Sample Exhibit",
                Description = "A sample exhibit in the sample city of Sampleborn",
            }));

            void Add<T>(T viewModel) => _viewModels[typeof(T)] = viewModel;
        }
    }
}
