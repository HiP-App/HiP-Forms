using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
    {

    [XamlCompilation (XamlCompilationOptions.Compile)]
    public partial class ExhibitPreviewView : ContentPage, IViewFor<ExhibitPreviewViewModel>
        {
        public ExhibitPreviewView ()
            {
            InitializeComponent ();
            }
        }
    }
