using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using HipMobileUI.Annotations;

namespace HipMobileUI.Viewmodels.DialogViewmodels
{
    class CaptionReferenceViewmodel : INotifyPropertyChanged {

        private List<Source> references;

        private string referenceTitle;
        private string referenceText;

        public void Init (List<Source> references)
        {
            References = references;
        }


        public string ReferenceTitle { get; set; }

        public string ReferenceText { get; set; }

        public List<Source> References {
            get { return references; }
            set {
                references = value;
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
