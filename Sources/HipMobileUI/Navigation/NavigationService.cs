using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HipMobileUI.Viewmodels;
using Xamarin.Forms;

namespace HipMobileUI.Navigation
{
    public class NavigationService : INavigationService, IViewCreator {

        #region Singleton
        public NavigationService ()
        {
        }

        private static NavigationService _instance;

        public static NavigationService Instance {
            get {
                if (_instance == null)
                {
                    _instance= new NavigationService ();
                }
                return _instance;
            }
        }
        #endregion

        readonly Dictionary<Type, Type> _viewModelViewDictionary = new Dictionary<Type, Type>();

        INavigation FormsNavigation
        {
            get
            {
                var tabController = Application.Current.MainPage as TabbedPage;
                var masterController = Application.Current.MainPage as MasterDetailPage;

                // First check to see if we're on a tabbed page, then master detail, finally go to overall fallback
                return tabController?.CurrentPage?.Navigation ??
                                     (masterController?.Detail as TabbedPage)?.CurrentPage?.Navigation ?? // special consideration for a tabbed page inside master/detail
                                     masterController?.Detail?.Navigation ??
                                     Application.Current.MainPage.Navigation;
            }
        }

        public async Task PopAsync (bool animate=false)
        {
            await FormsNavigation.PopAsync(animate);
        }

        public async Task PopModalAsync (bool animate=false)
        {
            await FormsNavigation.PopModalAsync(animate);
        }

        public async Task PushAsync (NavigationViewModel viewModel, bool animate=false)
        {
            var view = InstantiateView(viewModel);

            await FormsNavigation.PushAsync((Page)view, animate);
        }

        public async Task PushModalAsync (NavigationViewModel viewModel, bool animate=false)
        {
            var view = InstantiateView(viewModel);

            // Most likely we're going to want to put this into a navigation page so we can have a title bar on it
            var nv = new NavigationPage((Page)view);

            await FormsNavigation.PushModalAsync(nv, animate);
        }

        public async Task PopToRootAsync (bool animate=false)
        {
            await FormsNavigation.PopToRootAsync(animate);
        }

        public void RegisterViewModels (Assembly asm)
        {
            // Loop through everything in the assembley that implements IViewFor<T>
            foreach (var type in asm.DefinedTypes.Where(dt => !dt.IsAbstract &&
                dt.ImplementedInterfaces.Any(ii => ii == typeof(IViewFor))))
            {

                // Get the IViewFor<T> portion of the type that implements it
                var viewForType = type.ImplementedInterfaces.FirstOrDefault(
                    ii => ii.IsConstructedGenericType &&
                    ii.GetGenericTypeDefinition() == typeof(IViewFor<>));

                // Register it, using the T as the key and the view as the value
                Register(viewForType.GenericTypeArguments[0], type.AsType());
            }
        }

        public void Register(Type viewModelType, Type viewType)
        {
            _viewModelViewDictionary.Add(viewModelType, viewType);
        }

        public IViewFor InstantiateView(NavigationViewModel viewModel)
        {
            // Figure out what type the view model is
            var viewModelType = viewModel.GetType();

            // look up what type of view it corresponds to
            var viewType = _viewModelViewDictionary[viewModelType];

            // instantiate it
            var view = (IViewFor)Activator.CreateInstance(viewType);

            (view as BindableObject).BindingContext= viewModel;

            return view;
        }

    }
}
