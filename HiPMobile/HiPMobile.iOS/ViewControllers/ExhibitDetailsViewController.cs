using Foundation;
using System;
using UIKit;

namespace HiPMobile.iOS
{
    public partial class ExhibitDetailsViewController : UIViewController
    {
        public string ExhibitID { get; set; }
        public string ExhibitTitle { get; set; }


        public ExhibitDetailsViewController (IntPtr handle) : base (handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PagesCollectionViewSource source = new PagesCollectionViewSource();
            this.pagesCollectionView.Source = source;
        }


        private class PagesCollectionViewSource : UICollectionViewSource
        {
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                UICollectionViewCell cell = collectionView.DequeueReusableCell("exhibitDetailPage", indexPath) as UICollectionViewCell;
                cell.BackgroundColor = UIColor.FromRGB(indexPath.Row * indexPath.Row * 150, indexPath.Row + indexPath.Row * 100 , indexPath.Row * 50 / 2);
                return cell;
            }

            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return 0;
            }

            
        }
    }
}