using CoreAnimation;
using Foundation;
using HiPMobile.iOS;
using System;
using UIKit;

// slide out feature with the help of http://www.appliedcodelog.com/2015/09/sliding-menu-in-xamarinios-using.html

namespace HiPMobile.iOS
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController (IntPtr handle) : base (handle)
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
        }

        partial void TapMenuBarButton(UIBarButtonItem sender)
        {
            PerformTableTransition();
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
                PerformTableTransition();
            }
        }

        void SwipeRightToLeft()
        {
            if (!menuTableView.Hidden)
            {
                PerformTableTransition();
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

        //actions baed on the menu item selection
        void MenuSelected(string menuSelected)
        {
            SwipeRightToLeft();
        }
    }

    public class MenuTableViewSource : UITableViewSource
    {
        public event Action<string> MenuSelected;
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Constants.menuItems.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MenuTableViewCell cell = tableView.DequeueReusableCell(MenuTableViewCell.key) as MenuTableViewCell;
            //if (cell == null)
            //{
            //    cell = new UITableViewCell(UITableViewCellStyle.Default, MenuTableViewCell.key) as MenuTableViewCell;
            //}
            cell.InitCell(Constants.menuItems[indexPath.Row], Constants.menuItemsImages[indexPath.Row]);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (MenuSelected != null)
            {
                MenuSelected(Constants.menuItems[indexPath.Row]);
            }
            tableView.DeselectRow(indexPath, true);
        }

    }
}