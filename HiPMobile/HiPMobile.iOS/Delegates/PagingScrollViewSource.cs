using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace HiPMobile.iOS
{
    public abstract class PagingScrollViewSource : UIScrollViewDelegate
    {
        private nint page = 0;
        private UIView previousPageView;
        private UIView currentPageView;
        private UIView nextPageView;

        public nint CurrentPage
        {
            get
            {
                return page;
            }
        }

        public void LoadInitialViews(UIScrollView scrollView)
        {
            nfloat scrollViewWidth = scrollView.Frame.Width;
            nfloat scrolViewHeight = scrollView.Frame.Height;

            previousPageView = GetPageView(scrollView, 0);
            if (previousPageView != null)
            {
                previousPageView.Frame = new CoreGraphics.CGRect(0, 0, scrollViewWidth, scrolViewHeight);
                scrollView.AddSubview(previousPageView);
            }
            currentPageView = GetPageView(scrollView, 1);
            if (currentPageView != null)
            {
                currentPageView.Frame = new CoreGraphics.CGRect(scrollViewWidth, 0, scrollViewWidth, scrolViewHeight);
                scrollView.AddSubview(currentPageView);
            }

            nextPageView = GetPageView(scrollView, 2);
            if (nextPageView != null)
            {
                nextPageView.Frame = new CoreGraphics.CGRect(scrollViewWidth * 2, 0, scrollViewWidth, scrolViewHeight);
                scrollView.AddSubview(nextPageView);
            }
        }
        // define optional(virtual) and required(abstract) methods
        //optional methods
        public override void DecelerationEnded(UIScrollView scrollView)
        {
            nfloat pageWidth = scrollView.Frame.Width;
            nfloat pageHeight = scrollView.Frame.Height;
            nint currentPage = (nint) Math.Floor((scrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;

            if (currentPage > 1 && currentPage < NumberOfPages() - 1)// -1 or -2 depends on the first page == 0 or ==1
            {
                if (page < currentPage) 
                {
                    currentPageView = nextPageView;
                    nextPageView = GetPageView(scrollView, currentPage + 1);
                    nextPageView.Frame = new CoreGraphics.CGRect(pageWidth * (currentPage + 1), 0, pageWidth , pageHeight);
                    scrollView.AddSubview(nextPageView);
                }
                else
                {
                    if (page > currentPage)
                    {
                        currentPageView = previousPageView;
                        previousPageView = GetPageView(scrollView, currentPage - 1);
                        previousPageView.Frame = new CoreGraphics.CGRect(pageWidth * (currentPage - 1), 0, pageWidth, pageHeight);
                        scrollView.AddSubview(previousPageView);
                    }
                }
            }

            page = currentPage;
        }

        //required methods
        public abstract nint NumberOfPages();
        public abstract UIView GetPageView(UIScrollView scrollView, nint page);

     }
}
