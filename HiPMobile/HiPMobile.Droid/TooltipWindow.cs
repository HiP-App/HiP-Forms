using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace de.upb.hip.mobile.droid
{
    [Activity(Label = "TooltipWindow")]
    public class TooltipWindow
    {

        private const int MSG_DISMISS_TOOLTIP = 100;
        Context ctx;
        protected PopupWindow tipWindow;
        private ViewGroupOverlay viewGroupOverlay;
        View contentView;
        View contentView2;
        LayoutInflater inflater;


        public TooltipWindow(Context ctx)
        {
            this.ctx = ctx;

            tipWindow = new PopupWindow(ctx);
            inflater = (LayoutInflater) ctx.GetSystemService(Context.LayoutInflaterService);
            contentView = inflater.Inflate(Resource.Layout.custom_tooltip, null);

        }
        /*
       protected override void OnCreate (Bundle bundle)
       {
           base.OnCreate (bundle);
           FrameLayout overlayFramelayout = new FrameLayout(ctx);
           overlayFramelayout.Background = (Drawable)Android.Resource.Color.Black;
           //SetContentView(overlayFramelayout);
           //View view = LayoutInflater.Inflate()

           tipWindow = new PopupWindow(ctx);
           inflater = (LayoutInflater)ctx.GetSystemService(Context.LayoutInflaterService);
           //contentView = inflater.Inflate (Resource.Layout.custom_tooltip, null);
           contentView = inflater.Inflate(Resource.Layout.custom_tooltip, overlayFramelayout, false);
           overlayFramelayout.AddView(contentView);
       }*/

        public void showToolTip(View anchor)
        {
            Console.WriteLine("USAO BREE");
            tipWindow.Height = ViewGroup.LayoutParams.WrapContent; //it was actionbar before
            tipWindow.Width = ViewGroup.LayoutParams.WrapContent;
            tipWindow.OutsideTouchable = false;
            tipWindow.Focusable = true;
            tipWindow.SetBackgroundDrawable(new BitmapDrawable());
            //tipWindow.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            tipWindow.ContentView = contentView;

            tipWindow.AnimationStyle = Resource.Style.animationName;

            /*
            Rect anchor_rect = new Rect(positionX, positionY, positionX + positionY, positionY + positionY);

            contentView.Measure(ActionBar.LayoutParams.WrapContent, ActionBar.LayoutParams.WrapContent);

            int contentViewheight = contentView.MeasuredHeight;
            int contentViewwidth = contentView.MeasuredWidth;

            int position_x = anchor_rect.CenterX() - (contentViewwidth / 2);
            int position_y = anchor_rect.Bottom - (anchor_rect.Height() / 2);

            //tipWindow.ShowAtLocation(anchor, GravityFlags.NoGravity, position_x, position_y);

            */
            int[] screen_pos = new int[2];
            anchor.GetLocationOnScreen(screen_pos);
            Rect anchor_rect = new Rect(screen_pos[0], screen_pos[1], screen_pos[0] + anchor.Width, screen_pos[1] + anchor.Height);

            contentView.Measure(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            int contentViewheight = contentView.MeasuredHeight;
            int contentViewwidth = contentView.MeasuredWidth;

            

            int position_x = anchor_rect.CenterX() - (contentViewwidth / 2);
            int position_y = anchor_rect.Bottom - (anchor_rect.Height() / 2);

            tipWindow.ShowAtLocation(anchor, GravityFlags.NoGravity, position_x, position_y);
           // tipWindow.ShowAtLocation(anchor, GravityFlags.Top, 0, 1300);

            Console.WriteLine("Anchor: " + screen_pos[0] + " pos 1 " + screen_pos[1] + "contentViewHeigth" + contentViewheight + "contenttViewWidth" + contentViewwidth);
            Handler h = new MyHandler(this);
            //h.SendEmptyMessageDelayed(MSG_DISMISS_TOOLTIP, 4000);

        }

        public void ShowPopUp(View anchor, int position_x, int position_y)
        {
            tipWindow.ShowAtLocation(anchor, GravityFlags.NoGravity, position_x, position_y);
        }

        public bool IsTooltipShown()
        {
            if (tipWindow != null && tipWindow.IsShowing)
                return true;
            return false;
        }

        public void DismissToolTip()
        {
            if (tipWindow != null && tipWindow.IsShowing)
                tipWindow.Dismiss();
        }

        private class MyHandler : Handler
        {

            private TooltipWindow parent;

            public MyHandler(TooltipWindow parent)
            {
                this.parent = parent;
            }
            public override void HandleMessage(Android.OS.Message msg)
            {
                switch (msg.What)
                {
                    case MSG_DISMISS_TOOLTIP:
                        if (parent.tipWindow != null && parent.tipWindow.IsShowing)
                            parent.tipWindow.Dismiss();
                        break;

                }
            }


        }

    };

}
