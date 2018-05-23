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
using System.Collections.ObjectModel;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Container
{
    /// <summary>
    /// View for displaying a collection of tabs. The tabs are shown at the top and can be clicked to switch the content below.
    /// </summary>
    public class TabContainerView : ContentView
    {
        private readonly Grid header;
        private readonly ContentView contentContainer;

        public TabContainerView()
        {
            var layout = new StackLayout() { Orientation = StackOrientation.Vertical, Padding = new Thickness(0, 0) };

            header = new Grid();
            header.RowSpacing = 0;
            header.ColumnSpacing = 0;
            header.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45, GridUnitType.Absolute) });

            contentContainer = new ContentView() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            layout.Children.Add(header);
            layout.Children.Add(contentContainer);

            Content = layout;

            TabViews = new ObservableCollection<View>();
        }

        public static readonly BindableProperty TabsProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(ObservableCollection<string>),
            typeof(TabContainerView),
            propertyChanged: TabsPropertyChanged
        );

        /// <summary>
        /// Collection of strings to be displayed as tab titles.
        /// </summary>
        public ObservableCollection<string> Tabs
        {
            get { return (ObservableCollection<string>)GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        /// <summary>
        /// Gets called when the <see cref="Tabs"/> property changes.
        /// Clears the tab bar and adds a new tab (with divider) for each string
        /// in the <see cref="Tabs"/> collection. Sets all children of the <see cref="TabContainerView"/> 
        /// to invisible.
        /// </summary>
        private static void TabsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = bindable as TabContainerView;
            var newTabs = newValue as ObservableCollection<string>;

            if (tabView == null || newTabs == null)
                return;

            var tabBar = tabView.header;

            // replace current tabs with new ones
            tabBar.Children.Clear();
            tabBar.ColumnDefinitions.Clear();

            for (int i = 0; i < newTabs.Count; i++)
            {
                tabBar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                tabBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1) });
                var title = newTabs.ElementAt(i);
                tabBar.Children.Add(CreateTab(tabView, title, i), 2 * i, 0);
                tabBar.Children.Add(CreateDivider(), 2 * i + 1, 0);
            }

            tabBar.Children.RemoveAt(tabBar.Children.Count - 1); // remove last divider

            // reset current tab to first
            tabView.CurrentTab = "0";
        }

        public static readonly BindableProperty CurrentTabProperty = BindableProperty.Create(
            nameof(CurrentTab),
            typeof(string),
            typeof(TabContainerView),
            propertyChanged: CurrentTabPropertyChanged
        );

        /// <summary>
        /// The currently selected tab of the <see cref="TabContainerView"/>.
        /// </summary>
        public string CurrentTab
        {
            get { return (string)GetValue(CurrentTabProperty); }
            set { SetValue(CurrentTabProperty, value); }
        }

        /// <summary>
        /// Gets called when the currently selected tab changes.
        /// Removes the highlight from the previously selected tab and sets its child to invisible.
        /// Adds the highlight to the newly selected tab and sets its child to visible.
        /// </summary>
        private static void CurrentTabPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabView = bindable as TabContainerView;

            var tabBar = tabView?.header;
            if (tabBar == null)
                return;

            // reset previous index
            if (oldvalue != null)
            {
                var previousTabIndex = int.Parse((string)oldvalue);
                var tab = tabBar.Children.ElementAt(TranslateTabIndexToLabelIndex(previousTabIndex));
                UnhighlightTabLabel(tab as Label);
            }

            // set new index
            if (newvalue != null)
            {
                var currentTabIndex = int.Parse((string)newvalue);
                HighlightTabLabel(tabBar.Children.ElementAt(TranslateTabIndexToLabelIndex(currentTabIndex)) as Label);

                SetDisplayedView(tabView, TranslateTabIndexToChildIndex(currentTabIndex));
            }
        }

        public static readonly BindableProperty TabViewsProperty = BindableProperty.Create(
            nameof(TabViews),
            typeof(ObservableCollection<View>),
            typeof(TabContainerView),
            new ObservableCollection<View>()
        );

        /// <summary>
        /// The collection of the <see cref="TabContainerView"/>.
        /// </summary>
        public ObservableCollection<View> TabViews
        {
            get { return (ObservableCollection<View>)GetValue(TabViewsProperty); }
            set { SetValue(TabViewsProperty, value); }
        }

        /// <summary>
        /// Creates a new tab with the specified title which will be 
        /// associated with the specified index of the specified <see cref="TabContainerView"/>.
        /// </summary>
        /// <param name="tabView">The <see cref="TabContainerView"/> the tab is created for.</param>
        /// <param name="title">The displayed title of the tab.</param>
        /// <param name="index">The index of the container to show when the tab is selected.</param>
        /// <returns>A <see cref="Label"/> representing the newly created tab.</returns>
        private static View CreateTab(TabContainerView tabView, string title, int index)
        {
            var lbl = new Label
            {
                Text = title,
                FontSize = 20,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(6, 0)
            };
            lbl.SetDynamicResource(StyleProperty, "TitleStyle");

            // change tab on click
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (s, e) => { tabView.CurrentTab = $"{index}"; };
            lbl.GestureRecognizers.Add(gestureRecognizer);

            return lbl;
        }

        /// <summary>
        /// Highlights the specified tab label by adjusting its appeareance.
        /// </summary>
        /// <param name="tab">The tab to highlight.</param>
        private static void HighlightTabLabel(Label tab)
        {
            var resources = IoCManager.Resolve<ApplicationResourcesProvider>();
            tab.TextColor = resources.TryGetResourceColorvalue("PrimaryColor");
        }

        /// <summary>
        /// Removes any highlight from the specified tab.
        /// </summary>
        /// <param name="tab">The tab which should be stripped of the highlight.</param>
        private static void UnhighlightTabLabel(Label tab)
        {
            tab.TextColor = Color.Black;
        }

        /// <summary>
        /// Creates a new divider which is displayed between tabs.
        /// </summary>
        /// <returns>A <see cref="BoxView"/> with one unit width.</returns>
        private static View CreateDivider()
        {
            return new BoxView
            {
                Color = Color.Gray,
                WidthRequest = 1,
                Margin = new Thickness(0, 6)
            };
        }

        /// <summary>
        /// Translates a tab index to the position of the corresponding
        /// <see cref="Label"/> in the tab bar. Necessary to skip dividers.
        /// </summary>
        /// <param name="index">The tab index.</param>
        /// <returns>The index of the corresponding Label in the tab bar.</returns>
        private static int TranslateTabIndexToLabelIndex(int index)
        {
            return index * 2;
        }

        /// <summary>
        /// Translates a tab index to the index of the corresponding
        /// container within the children of the <see cref="TabContainerView"/>. 
        /// Necessary to skip the tab bar.
        /// </summary>
        /// <param name="index">The tab index.</param>
        /// <returns>The index of the corresponding child.</returns>
        private static int TranslateTabIndexToChildIndex(int index)
        {
            return index;
        }

        /// <summary>
        /// Sets the currently displayed view by index.
        /// </summary>
        /// <param name="control">The instance of the <see cref="TabContainerView"/>.</param>
        /// <param name="index">The index to set.</param>
        private static void SetDisplayedView(TabContainerView control, int index)
        {
            if (control == null || index < 0 || index >= control.TabViews.Count)
            {
                throw new ArgumentException("Illegal arguments passed.");
            }
            control.contentContainer.Content = control.TabViews[index];
        }
    }
}