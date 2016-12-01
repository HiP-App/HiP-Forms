using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Annotations;

namespace HipMobileUI.Viewmodels.ExhibitDetailsViewmodels
{
    class CaptionTextViewmodel : INotifyPropertyChanged
    {

        private string formatedText;

        public void Init (string formatedText)
        {
            FormatedText = formatedText;
        }

        public string FormatedText
        {
            get
            {
                return formatedText;
            }

            set
            {
                formatedText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}
