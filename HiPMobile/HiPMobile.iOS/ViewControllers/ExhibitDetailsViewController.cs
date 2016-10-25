using Foundation;
using System;
using UIKit;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using System.Collections.Generic;

namespace HiPMobile.iOS
{
    public partial class ExhibitDetailsViewController : UIViewController
    {
        public Exhibit Exhibit { get; set; }        
        public ExhibitDetailsViewController (IntPtr handle) : base (handle)
        {

        }

        public override void ViewDidLoad()
        {
            exhibitDetailsScrollView.Frame = new CoreGraphics.CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            exhibitDetailsScrollView.ContentSize = new CoreGraphics.CGSize(exhibitDetailsScrollView.Frame.Size.Width * Exhibit.Pages.Count, exhibitDetailsScrollView.Frame.Size.Height);
            exhibitDetailsScrollView.BackgroundColor = UIColor.Black;
            base.ViewDidLoad();
            ScrollViewSource scrollViewSource = new ScrollViewSource();
            scrollViewSource.DetailPages = Exhibit.Pages;
           // scrollViewSource.PageChanged += PageChanged;
            exhibitDetailsScrollView.Delegate = scrollViewSource;
            scrollViewSource.LoadInitialViews(exhibitDetailsScrollView);
        }

        //void PageChanged(nint page)
        //{
        //    NavigationItem.Title = page.ToString();
        //}


        private class ScrollViewSource : PagingScrollViewSource
        {
            public IList<Page> DetailPages;
            //internal event Action<nint> PageChanged;

            public override nint NumberOfPages()
            {
                return DetailPages.Count;
            }
            public override UIView GetPageView(UIScrollView scrollView, nint index)
            {
                Page page = DetailPages[(int)index];

                //init view from xib instead this ->
                UIView pageView = new UIView();
                pageView.BackgroundColor = index % 2 == 0? UIColor.Yellow: UIColor.Purple;
                //<-init view from xib instead this

                return pageView;
            }

            public override void DecelerationEnded(UIScrollView scrollView)
            {
                base.DecelerationEnded(scrollView);
             //   PageChanged(CurrentPage);
            }
        }                   
    }
}