using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace HipMobileUI.Container
{

    public class TabView : StackLayout
    {
        private readonly StackLayout tabBar;

        public TabView ()
        {
            Orientation = StackOrientation.Vertical;

            tabBar = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = 45
            };
            Children.Add (tabBar);
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

            var tabBar = tabView.tabBar;

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

            // hide all children except tab bar
            foreach (var child in tabView.Children)
            {
                child.IsVisible = false;
            }
            tabView.Children.First ().IsVisible = true;

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
                
        public string CurrentTab {
            get { return (string)GetValue(CurrentTabProperty); }
            set { SetValue (CurrentTabProperty, value); }
        }
        
        private static void CurrentTabPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabView = bindable as TabView;
            
            var tabBar = tabView?.Children.First() as StackLayout;
            if (tabBar == null)
                return;

            // reset previous index
            if (oldvalue != null)
            {
                var previousTabIndex = int.Parse((string)oldvalue);
                var tab = tabBar.Children.ElementAt (TranslateTabIndexToLabelIndex (previousTabIndex));
                UnhighlightTabLabel(tab as Label);

                var contents = tabView.Children.ElementAt (TranslateTabIndexToChildIndex (previousTabIndex));
                contents.IsVisible = false;
            }

            // set new index
            if (newvalue != null)
            {
                var currentTabIndex = int.Parse((string)newvalue);
                HighlightTabLabel(tabBar.Children.ElementAt (TranslateTabIndexToLabelIndex(currentTabIndex)) as Label);

                var contents = tabView.Children.ElementAt (TranslateTabIndexToChildIndex (currentTabIndex));
                contents.IsVisible = true;
            }
            
        }

        #endregion
        

        // creates a new tab with the given name
        private static View CreateTab(TabView tabView, string title, int index)
        {
            var lbl = new Label
            {
                Text = title,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(8, 0)
            };

            lbl.SetDynamicResource (StyleProperty, "TitleStyle");

            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (s, e) => {
                tabView.CurrentTab = $"{index}";
            };
            lbl.GestureRecognizers.Add (gestureRecognizer);

            return lbl;
        }

        private static void HighlightTabLabel (Label tab)
        {

            tab.TextColor = (Color) Application.Current.Resources ["AccentColor"];
        }

        private static void UnhighlightTabLabel (Label tab)
        {
            tab.TextColor = Color.Black;
        }

        // creates a new divider
        private static View CreateDivider()
        {
            return new BoxView
            {
                Color = Color.Gray,
                WidthRequest = 1,
                Margin = new Thickness(0, 6)
            };
        }

        private static int TranslateTabIndexToLabelIndex (int index)
        {
            return index * 2;
        }

        private static int TranslateTabIndexToChildIndex (int index)
        {
            return index + 1;
        }

    }
}