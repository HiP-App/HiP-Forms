using System.ComponentModel;
using System.Runtime.CompilerServices;
using de.upb.hip.mobile.pcl.Common;
using HipMobileUI.Annotations;
using HipMobileUI.Navigation;
using Microsoft.Practices.Unity;

namespace HipMobileUI.Viewmodels
{
    public abstract class BaseViewModel : INotifyPropertyChanged {

        protected static INavigationService Navigation = IoCManager.UnityContainer.Resolve<INavigationService> ();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}
