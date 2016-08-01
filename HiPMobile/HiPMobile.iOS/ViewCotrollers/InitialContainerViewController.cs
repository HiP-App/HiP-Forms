using Foundation;
using System;
using UIKit;

using FlyoutNavigation;
using MonoTouch.Dialog;
using System.Linq;

namespace HiPMobile.iOS
{
    public partial class InitialContainerViewController : UIViewController
    {
        FlyoutNavigationController navigationMenu;

        public InitialContainerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            navigationMenu = new FlyoutNavigationController();
            navigationMenu.Position = FlyOutNavigationPosition.Left;
            navigationMenu.View.Frame = this.containerView.Frame;
            this.containerView.AddSubview(navigationMenu.View);
            this.AddChildViewController(navigationMenu);

            //foreach (string item in HipConstants.MenuItems)
            //{
            //    NavigationItem = item;
            //}
            //navigationMenu.NavigationItem

            navigationMenu.NavigationRoot = new RootElement("Menu") {
                new Section () {
                    from item in HipConstants.menuItems
                        select new StringElement (item) as Element
                }
            };

            navigationMenu.ViewControllers = Array.ConvertAll(HipConstants.viewControllers, controller => Storyboard.InstantiateViewController(controller));
        }

        //partial void MenuBarButtonItem_Activated(UIBarButtonItem sender)
        //{
        //    navigationMenu.ShowMenu();
        //}

        //public void ShowMenu()
        //{
        //    navigationMenu.ShowMenu();
        //}
    }
}