using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    public partial class ExhibitsOverviewView : IViewFor<ExhibitsOverviewViewModel>
    {
        public ExhibitsOverviewView()
        {
            InitializeComponent();
        }

        private void ExhibitCarousel_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var viewModel = (ExhibitsOverviewListItemViewModel) e.SelectedItem;
            var exhibit = viewModel.Exhibit;
            if (BindingContext is ExhibitsOverviewViewModel vm)
            {

                vm.ExhibitChanged(exhibit);
            }
        }
    }
}