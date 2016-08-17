using Foundation;
using System;
using UIKit;
using MapKit;

namespace HiPMobile.iOS
{
    public partial class HomeScreenViewController : UIViewController
    {
        public HomeScreenViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //map
            MapViewDelegate mapDelegate = new MapViewDelegate();
            mapView.Delegate = mapDelegate;

            string template = "http://tile.openstreetmap.org/{0}/{1}/{2}.png/";
            MKTileOverlay overlay = new MKTileOverlay(template);
            overlay.CanReplaceMapContent = true;
            mapView.AddOverlay(overlay, MKOverlayLevel.AboveLabels);

            //tableView
            exhibitsTableView.RegisterNibForCellReuse(UINib.FromName("ExhibitTableViewCell", null),
                ExhibitTableViewCell.key);
            exhibitsTableView.Source = new ExhibitsTableViewSource();

        }

        private class MapViewDelegate : MKMapViewDelegate
        {


            public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
            {
                if (overlay is MKTileOverlay)
                {
                    var renderer = new MKTileOverlayRenderer((MKTileOverlay) overlay);
                    return renderer;
                }
                else
                {
                    return null;
                }
            }
        }

        private class ExhibitsTableViewSource : UITableViewSource
        {
            public ExhibitCellViewModel[] Exhibits { get; set; }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                ExhibitTableViewCell cell =
                    tableView.DequeueReusableCell(ExhibitTableViewCell.key) as ExhibitTableViewCell;
                cell.SetUpimageAppearance();
                cell.BackgroundColor = UIColor.Cyan;
                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (Exhibits != null)
                {
                    return Exhibits.Length;
                }

                return 2;
            }
        }
    }
}