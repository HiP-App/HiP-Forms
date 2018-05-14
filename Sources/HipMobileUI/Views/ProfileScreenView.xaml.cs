using PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileScreenView : IViewFor<ProfileScreenViewModel>
    {
        private DeviceOrientation orientation;

        public ProfileScreenView()
        {
            InitializeComponent();
            DesignMode.Initialize(this);
            orientation = DeviceOrientation.Undefined;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height && orientation != DeviceOrientation.Landscape)
            {
                orientation = DeviceOrientation.Landscape;
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 0);
                OuterGrid.Children.Remove(TabContainer);
                OuterGrid.Children.Add(TabContainer, 1, 0);
            }
            else if (width < height && orientation != DeviceOrientation.Portrait)
            {
                orientation = DeviceOrientation.Portrait;
                OuterGrid.ColumnDefinitions.Clear();
                OuterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Clear();
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                OuterGrid.Children.Remove(InnerGrid);
                OuterGrid.Children.Add(InnerGrid, 0, 0);
                OuterGrid.Children.Remove(TabContainer);
                OuterGrid.Children.Add(TabContainer, 0, 1);
            }
        }
    }
}