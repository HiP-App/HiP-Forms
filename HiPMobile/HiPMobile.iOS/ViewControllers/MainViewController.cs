using CoreAnimation;
using Foundation;
using HiPMobile.iOS;
using System;
using CoreGraphics;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using MediaToolbox;
using Microsoft.Practices.Unity;
using UIKit;

// slide out feature with the help of http://www.appliedcodelog.com/2015/09/sliding-menu-in-xamarinios-using.html

namespace HiPMobile.iOS
{
    public partial class MainViewController : UIViewController
    {
        private UIViewController containerViewController;

        public MainViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MenuTableViewSource menuTableViewSource = new MenuTableViewSource();
            menuTableView.Source = menuTableViewSource;
            menuTableViewSource.MenuSelected += MenuSelected;
            InitializeView();
            menuTableView.Hidden = true;
            shadowView.Hidden = true;

            this.navigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White
            };

            IMainScreenContainable iViewController = (IMainScreenContainable)containerViewController;
            this.navigationItem.Title = iViewController.NavigationTitle; //Have no idea why the standard NavigationItem poperty of the ViewController doesnt relate to the navigation bar

            //// Move this block to the Launch screen - Moved to app delegate
            //IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess>();
            //IoCManager.UnityContainer.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            //IoCManager.UnityContainer.RegisterType<IImageDimension, IosImageDimensions> ();
            //DbManager.UpdateDatabase ();
            //// End of block
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            if (segue.Identifier.Equals("mainScreenEmbedSegue"))
            {
                containerViewController = segue.DestinationViewController;
            }
        }

        //unwind segue
        [Action ("UnwindToMainViewController:")]
        public void UnwindToMainViewController (UIStoryboardSegue segue)
        {

        }

        partial void TapMenuBarButton(UIBarButtonItem sender)
        {
            PerformTableTransition();
            shadowView.Hidden = !shadowView.Hidden;
        }

        void InitializeView()
        {
            //custom swipe gesture recognizer
            var recognizerRight = new UISwipeGestureRecognizer(SwipeLeftToRight);
            recognizerRight.Direction = UISwipeGestureRecognizerDirection.Right;
            View.AddGestureRecognizer(recognizerRight);

            var recognizerLeft = new UISwipeGestureRecognizer(SwipeRightToLeft);
            recognizerLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            View.AddGestureRecognizer(recognizerLeft);
        }

        void SwipeLeftToRight()
        {
            if (menuTableView.Hidden)
            {
                shadowView.Hidden = false;
                PerformTableTransition();
            }
        }

        void SwipeRightToLeft()
        {
            if (!menuTableView.Hidden)
            {
                PerformTableTransition();
                shadowView.Hidden = true;
            }
        }

        void PerformTableTransition()
        {
            menuTableView.Hidden = !menuTableView.Hidden;
            //transition effect
            var transition = new CATransition();
            transition.Duration = 0.25f;
            transition.Type = CAAnimation.TransitionPush;
            if (menuTableView.Hidden)
            {
                transition.TimingFunction = CAMediaTimingFunction.FromName(new NSString("easeOut"));
                transition.Subtype = CAAnimation.TransitionFromRight;
            }
            else
            {
                transition.TimingFunction = CAMediaTimingFunction.FromName(new NSString("easeIn"));
                transition.Subtype = CAAnimation.TransitionFromLeft;
            }
            menuTableView.Layer.AddAnimation(transition, null);

        }

        //actions based on the menu item selection
        void MenuSelected(NSIndexPath menuItemIndexPath)
        {
            if (
                !containerViewController.GetType()
                    .Name.Equals(Constants.menuItemsViewControllers[menuItemIndexPath.Row]))
            {
                //remove the current ViewController from the container view
                containerViewController.WillMoveToParentViewController(null);
                containerViewController.View.RemoveFromSuperview();
                containerViewController.RemoveFromParentViewController();

                //add new ViewController to the container view
                UIStoryboard mainStoryboard = UIStoryboard.FromName("Main", NSBundle.MainBundle);
                UIViewController viewController =
                    mainStoryboard.InstantiateViewController(Constants.menuItemsViewControllers[menuItemIndexPath.Row]);
                viewController.WillMoveToParentViewController(this);
                this.containerView.AddSubview(viewController.View);
                this.AddChildViewController(viewController);
                viewController.DidMoveToParentViewController(this);

                IMainScreenContainable iViewController = (IMainScreenContainable)viewController;
                this.navigationItem.Title = iViewController.NavigationTitle;
                
                containerViewController = viewController;
            }
            SwipeRightToLeft();
        }

        private class MenuTableViewSource : UITableViewSource
        {
            internal event Action<NSIndexPath> MenuSelected;
            private NSIndexPath selectedIndexPath = NSIndexPath.FromRowSection(0, 0); //home screen index

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return Constants.menuItems.Length;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                MenuTableViewCell cell = tableView.DequeueReusableCell(MenuTableViewCell.key) as MenuTableViewCell;
                cell.InitCell(Constants.menuItems[indexPath.Row], Constants.menuItemsImages[indexPath.Row]);
                if (indexPath.Equals(selectedIndexPath))
                {
                    cell.SetSelected(true, false);
                }
                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                if (MenuSelected != null)
                {
                    MenuSelected(indexPath);
                    selectedIndexPath = indexPath;
                }
            }

        }
    }
}