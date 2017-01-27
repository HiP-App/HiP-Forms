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

using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace HipMobileUI.Container
{
    /// <summary>
    /// A <see cref="Layout"/> which can display an arbitrary amount of tabs. 
    /// Since it is based on <see cref="StackLayout"/>, you can use the usual 
    /// properties like BackgroundColor etc.
    /// 
    /// <example> 
    /// This generic sample shows how to use the view. Refer to RouteDetailsPage.xaml 
    /// for a concrete example.
    /// <code><![CDATA[ 
    ///   <container:TabView Tabs="{Binding Tabs}"> <!-- Where Tabs is of type ObservableCollection<string>-->
    ///     <StackLayout>
    ///       <!-- content for first tab goes here -->
    ///     </StackLayout
    ///     <StackLayout>
    ///         <!-- content for second tab goes here -->
    ///     </StackLayout>
    ///     <!-- ... repeat for additional tabs -->
    ///   </TabView> 
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    public class TabView : StackLayout
    {
        /// <summary>
        /// The top bar containing the tabs.
        /// </summary>
        private readonly Grid tabBar;

        /// <summary>
        /// Creates a new <see cref="TabView"/>. Initializes an empty tab bar adapting to the tab height. 
        /// </summary>
        public TabView()
        {
            Orientation = StackOrientation.Vertical;

            tabBar = new Grid();
            tabBar.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Children.Add(tabBar);
        }

        #region Tabs

        public static readonly BindableProperty TabsProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(ObservableCollection<string>),
            typeof(TabView),
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
        /// in the <see cref="Tabs"/> collection. Sets all children of the <see cref="TabView"/> 
        /// to invisible.
        /// </summary>
        private static void TabsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = bindable as TabView;
            var newTabs = newValue as ObservableCollection<string>;

            if (tabView == null || newTabs == null)
                return;

            var tabBar = tabView.tabBar;

            // replace current tabs with new ones
            tabBar.Children.Clear();

            for (int i = 0; i < newTabs.Count; i++)
            {
                tabBar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                tabBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1) });
                var title = newTabs.ElementAt(i);
                tabBar.Children.Add(CreateTab(tabView, title, i), 2 * i, 0);
                tabBar.Children.Add(CreateDivider(), 2 * i + 1, 0);
            }

            tabBar.Children.RemoveAt(tabBar.Children.Count - 1);   // remove last divider

            // hide all children except tab bar
            foreach (var child in tabView.Children)
            {
                child.IsVisible = false;
            }
            tabView.Children.First().IsVisible = true;

            // reset current tab to first
            tabView.CurrentTab = "0";
        }

        #endregion

        #region CurrentTab

        public static readonly BindableProperty CurrentTabProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(string),
            typeof(TabView),
            propertyChanged: CurrentTabPropertyChanged
        );

        /// <summary>
        /// The currently selected tab of the <see cref="TabView"/>.
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
            var tabView = bindable as TabView;

            var tabBar = tabView?.Children.First() as Grid;
            if (tabBar == null)
                return;

            // reset previous index
            if (oldvalue != null)
            {
                var previousTabIndex = int.Parse((string)oldvalue);
                var tab = tabBar.Children.ElementAt(TranslateTabIndexToLabelIndex(previousTabIndex));
                UnhighlightTabLabel(tab as Label);

                var contents = tabView.Children.ElementAt(TranslateTabIndexToChildIndex(previousTabIndex));
                contents.IsVisible = false;
            }

            // set new index
            if (newvalue != null)
            {
                var currentTabIndex = int.Parse((string)newvalue);
                HighlightTabLabel(tabBar.Children.ElementAt(TranslateTabIndexToLabelIndex(currentTabIndex)) as Label);

                var contents = tabView.Children.ElementAt(TranslateTabIndexToChildIndex(currentTabIndex));
                contents.IsVisible = true;
            }

        }

        #endregion

        /// <summary>
        /// Creates a new tab with the specified title which will be 
        /// associated with the specified index of the specified <see cref="TabView"/>.
        /// </summary>
        /// <param name="tabView">The <see cref="TabView"/> the tab is created for.</param>
        /// <param name="title">The displayed title of the tab.</param>
        /// <param name="index">The index of the container to show when the tab is selected.</param>
        /// <returns>A <see cref="Label"/> representing the newly created tab.</returns>
        private static View CreateTab(TabView tabView, string title, int index)
        {
            var lbl = new Label
            {
                Text = title,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(6, 0)
            };
            lbl.SetDynamicResource(StyleProperty, "TitleStyle");

            // change tab on click
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (s, e) =>
            {
                tabView.CurrentTab = $"{index}";
            };
            lbl.GestureRecognizers.Add(gestureRecognizer);

            return lbl;
        }

        /// <summary>
        /// Highlights the specified tab label by adjusting its appeareance.
        /// </summary>
        /// <param name="tab">The tab to highlight.</param>
        private static void HighlightTabLabel(Label tab)
        {

            tab.TextColor = (Color)Application.Current.Resources["AccentColor"];
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
        /// container within the children of the <see cref="TabView"/>. 
        /// Necessary to skip the tab bar.
        /// </summary>
        /// <param name="index">The tab index.</param>
        /// <returns>The index of the corresponding child.</returns>
        private static int TranslateTabIndexToChildIndex(int index)
        {
            return index + 1;
        }

    }
}