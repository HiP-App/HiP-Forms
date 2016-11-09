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

namespace de.upb.hip.mobile.droid.Dialogs
{
    [Activity(Label = "TooltipWindow")]

    public class TooltipWindow
    {

        //private const int MSG_DISMISS_TOOLTIP = 100;
        Context ctx;
        protected PopupWindow tipWindow;
        private ViewGroupOverlay viewGroupOverlay;
        View contentView;
        LayoutInflater inflater;


        public TooltipWindow(Context ctx)
        {
            this.ctx = ctx;
            tipWindow = new PopupWindow(ctx);

            inflater = (LayoutInflater)ctx.GetSystemService(Context.LayoutInflaterService);
            contentView = inflater.Inflate(Resource.Layout.custom_tooltip, null);

        }

        public void showToolTip(View anchor)
        {
            tipWindow.Height = ActionBar.LayoutParams.WrapContent;
            tipWindow.Width = ActionBar.LayoutParams.WrapContent;
            tipWindow.OutsideTouchable = false;
            tipWindow.Focusable = true;
            tipWindow.SetBackgroundDrawable(new BitmapDrawable());
            //tipWindow.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            tipWindow.ContentView = contentView;

            tipWindow.AnimationStyle = Resource.Style.animationPopUp;

            int[] screen_pos = new int[2];
            anchor.GetLocationOnScreen(screen_pos);
            Console.WriteLine("POSITION: pos_0 " + screen_pos[0] + " pos_11" + screen_pos[1]);
            Rect anchor_rect = new Rect(screen_pos[0], screen_pos[1], screen_pos[0] + anchor.Width, screen_pos[1] + anchor.Height);

            contentView.Measure(ActionBar.LayoutParams.WrapContent, ActionBar.LayoutParams.WrapContent);

            int contentViewheight = contentView.MeasuredHeight;
            int contentViewwidth = contentView.MeasuredWidth;

            int position_x = anchor_rect.CenterX() - (contentViewwidth / 2);
            int position_y = anchor_rect.Bottom - (anchor_rect.Height() / 2);

            tipWindow.ShowAtLocation(anchor, GravityFlags.NoGravity, position_x, position_y);

            //Handler h = new MyHandler(this);
            //h.SendEmptyMessageDelayed(MSG_DISMISS_TOOLTIP, 4000);

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
        /*
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
        */
    };
}