using Foundation;
using System;
using CoreGraphics;
using CoreLocation;
using UIKit;
using MapKit;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using System.Collections.Generic;
//using de.upb.hip.mobile.pcl.DataAccessLayer;


namespace HiPMobile.iOS
{
    public partial class HomeScreenViewController : UIViewController
    {
        private List<ExhibitCellViewModel> exhibits;
        public HomeScreenViewController(IntPtr handle) : base(handle)
        {
            exhibits = new List<ExhibitCellViewModel>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //map
            MapViewDelegate mapDelegate = new MapViewDelegate();
            mapView.Delegate = mapDelegate;

            string template = "http://tile.openstreetmap.org/{z}/{x}/{y}.png";
            MKTileOverlay overlay = new MKTileOverlay(template);
            overlay.CanReplaceMapContent = true;
            mapView.AddOverlay(overlay, MKOverlayLevel.AboveLabels);

            // Center the map, for development purposes
            MKCoordinateRegion region = mapView.Region;
            region.Span.LatitudeDelta = 0.05;
            region.Span.LongitudeDelta = 0.05;
            region.Center = new CLLocationCoordinate2D(51.7166700, 8.7666700);
            mapView.Region = region;

            // Disable rotation programatically because value of designer is somehow ignored
            mapView.RotateEnabled = false;

            //tableView
            this.exhibitsTableView.RowHeight = 44;
            exhibitsTableView.RegisterNibForCellReuse(UINib.FromName("ExhibitTableViewCell", null),
                ExhibitTableViewCell.key);

            ExhibitsTableViewSource source = new ExhibitsTableViewSource();
            source.Exhibits = this.LoadExhibitsData();
            exhibitsTableView.Source = source;

            

        }

        private List<ExhibitCellViewModel> LoadExhibitsData()
        {
            List<ExhibitCellViewModel> exhibits = new List<ExhibitCellViewModel>();
            IEnumerable<Exhibit> exhibitsData = ExhibitManager.GetExhibits();
            foreach (Exhibit exhibit in exhibitsData)
            {
                ExhibitCellViewModel exhibitCellModel = new ExhibitCellViewModel(exhibit.Image, exhibit.Name);
                exhibits.Add(exhibitCellModel);
            }

            return exhibits;

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
            //public ExhibitCellViewModel[] Exhibits { get; set; }
            public List<ExhibitCellViewModel> Exhibits { get; set; }        
                        
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                ExhibitTableViewCell cell =
                    tableView.DequeueReusableCell(ExhibitTableViewCell.key) as ExhibitTableViewCell;
                cell.PopulateCell(Exhibits[indexPath.Row].Image, Exhibits[indexPath.Row].Name);                
                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (Exhibits != null)
                {
                    return Exhibits.Count;
                }

                return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 44;  
            }
        }
    }
}