using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HipMobileUI.Annotations;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.MainScreenContainables
{
    class DialogViewmodel : INotifyPropertyChanged {

        private string title;

        public DialogViewmodel ()
        {
            //OnAlertDialogDisplay = new Command(DisplayAlertDialog);
            //OnCaptionDialogDisplay = new Command(DisplayCaptionDialog);
        }

        public void DisplayAlertDialog ()
        {
            
        }

        public void DisplayCaptionDialog ()
        {
            
        }

        public string Title
        {
            get { return title; }
            set {
                title = value;
                OnPropertyChanged();
            }
        }

        //public ICommand OnAlertDialogDisplay { get; set; }
       // public ICommand OnCaptionDialogDisplay { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}
