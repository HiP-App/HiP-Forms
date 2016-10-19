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

            
            //this.exhibitDetailsScrollView.Frame = new CoreGraphics.CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            //this.exhibitDetailsScrollView.ContentSize = new CoreGraphics.CGSize(this.exhibitDetailsScrollView.Frame.Size.Width * 3, this.exhibitDetailsScrollView.Frame.Size.Height);
            //exhibitDetailsScrollView.PagingEnabled = true;
            //exhibitDetailsScrollView.BackgroundColor = UIColor.Black;
            //exhibitDetailsScrollView.Delegate = new ScrollViewSource();
            //Console.WriteLine("this.View.Frame.Height {0} ", this.View.Frame.Height);
            //Console.WriteLine("this.NavigationController.NavigationBar.Frame.Height {0} ", this.NavigationController.NavigationBar.Frame.Height);
            //Console.WriteLine("this.exhibitDetailsScrollView.Frame.Height {0}", this.exhibitDetailsScrollView.Frame.Height);
            //Console.WriteLine("this.View.Frame.Height {0}", this.View.Frame.Height);

            //nfloat scrollViewWidth = this.exhibitDetailsScrollView.Frame.Width;
            //nfloat scrolViewHeight = this.exhibitDetailsScrollView.Frame.Height;

            //UIView v1 = new UIView(new CoreGraphics.CGRect(0, 0, scrollViewWidth, scrolViewHeight));
            //v1.BackgroundColor = UIColor.Blue;
            //UIView v2 = new UIView(new CoreGraphics.CGRect(scrollViewWidth, 0, scrollViewWidth, scrolViewHeight));
            //v2.BackgroundColor = UIColor.Yellow;
            //UIView v3 = new UIView(new CoreGraphics.CGRect(scrollViewWidth * 2 , 0, scrollViewWidth, scrolViewHeight));
            //v3.BackgroundColor = UIColor.Red;

            //this.exhibitDetailsScrollView.AddSubview(v1);
            //this.exhibitDetailsScrollView.AddSubview(v2);
            //this.exhibitDetailsScrollView.AddSubview(v3);

            //Console.WriteLine("view ---------- {0} {1}",v1.Frame.X, v1.Frame.Y);

            //PagesCollectionViewSource source = new PagesCollectionViewSource();
            //this.pagesCollectionView.Source = source;
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