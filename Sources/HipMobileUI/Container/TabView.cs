using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace HipMobileUI.Container
{

    public class TabView : StackLayout
    {
        public TabView ()
        {
            Orientation = StackOrientation.Vertical;

//            foreach (var child in Children)
//            {
//                child.IsVisible = false;
//            }
        }

        #region Tabs

        public static readonly BindableProperty TabsProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(ObservableCollection<string>),
            typeof(TabView),
            propertyChanged: TabsPropertyChanged
        );

        public ObservableCollection<string> Tabs
        {
            get { return (ObservableCollection<string>)GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        private static void TabsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = bindable as TabView;
            var newTabs = newValue as ObservableCollection<string>;

            if (tabView == null || newTabs == null)
                return;

            // get current tab bar
            var tabBar = tabView.Children.First() as StackLayout;
            if (tabBar == null)
                return;

            // replace current tabs with new ones
            tabBar.Children.Clear();
            tabBar.Orientation = StackOrientation.Horizontal;

            for (int i = 0; i < newTabs.Count; i++)
            {
                var title = newTabs.ElementAt (i);
                tabBar.Children.Add(CreateTab(tabView, title, i));
                tabBar.Children.Add(CreateDivider());
            }

            tabBar.Children.RemoveAt(tabBar.Children.Count - 1);   // remove last divider

            // reset current tab to first
//            tabView.CurrentTab = 0;
        }

        #endregion

        #region CurrentTab

//        public static readonly BindableProperty CurrentTabProperty = BindableProperty.Create(
//            nameof(Tabs),
//            typeof(int),
//            typeof(TabView),
//            propertyChanged: CurrentTabPropertyChanged
//        );
//                
//        public int CurrentTab {
//            get { return (int)GetValue(CurrentTabProperty); }
//            set { SetValue (CurrentTabProperty, value); }
//        }
        
        private static void CurrentTabPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
//            var tabView = bindable as TabView;
//            var previousTabIndex = (int) oldvalue;
//            var currentTabIndex = (int) newvalue;
//
//            // update tab labels
//            var tabBar = tabView?.Children.First () as StackLayout;
//            if (tabBar == null)
//                return;
//
//            UnhilightTabLabel(tabBar.Children.ElementAt (previousTabIndex) as Label);
//            HighlightTabLabel(tabBar.Children.ElementAt (currentTabIndex) as Label);
//
//            // TODO: switch tab contents
        }

        #endregion
        

        // creates a new tab with the given name
        private static View CreateTab(TabView tabView, string title, int index)
        {
            var lbl = new Label
            {
                Text = title
            };

            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (s, e) => {
//                tabView.CurrentTab = index;
                Debug.WriteLine ("asdf");
            };

            lbl.GestureRecognizers.Add (gestureRecognizer);

            return lbl;
        }

        private static void HighlightTabLabel (Label tab)
        {
            tab.TextColor = Color.Orange;
            tab.FontAttributes = FontAttributes.Bold;
        }

        private static void UnhilightTabLabel (Label tab)
        {
            tab.TextColor = Color.Black;
            tab.FontAttributes = FontAttributes.None;
        }

        // creates a new divider
        private static View CreateDivider()
        {
            return new BoxView
            {
                Color = Color.Gray,
                WidthRequest = 1,
                Margin = new Thickness(4, 4, 4, 4)
            };
        }

    }
}