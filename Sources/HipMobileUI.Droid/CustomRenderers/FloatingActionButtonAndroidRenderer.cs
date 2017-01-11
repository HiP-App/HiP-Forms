using System;
using Android.Graphics.Drawables;
using Android.Util;
using de.upb.hip.mobile.droid.CustomRenderers;
using HipMobileUI.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FloatingActionButton), typeof(FloatingActionButtonAndroidRenderer))]
namespace de.upb.hip.mobile.droid.CustomRenderers
{
    public class FloatingActionButtonAndroidRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<FloatingActionButton, Android.Support.Design.Widget.FloatingActionButton> {

        private Android.Support.Design.Widget.FloatingActionButton fab;
        private FloatingActionButton currentButton;

        protected override void OnElementChanged (ElementChangedEventArgs<FloatingActionButton> e)
        {
            base.OnElementChanged (e);

            if (Control == null)
            {
                fab = new Android.Support.Design.Widget.FloatingActionButton(Context);
                SetNativeControl(fab);
                
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                
            }
            if (e.NewElement != null)
            {
                // Subscribe
                currentButton = e.NewElement;
                currentButton.IconChanged+=CurrentButtonOnIconChanged;
                fab.Click += FabOnClick;

                fab.SetBackgroundColor(e.NewElement.NormalColor.ToAndroid());
                CurrentButtonOnIconChanged (currentButton.Icon);
                var a=ConvertDpToPixel (48);
            }
        }

        public static float ConvertDpToPixel(float dp)
        {
            var metrics = Android.Content.Res.Resources.System.DisplayMetrics;
            return dp * ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);
        }

        private void CurrentButtonOnIconChanged (string newIcon)
        {
            //fab.SetImageDrawable (Resources.GetDrawable (Android.Resource.Drawable.ButtonPlus));
            int resourceId = Resources.GetIdentifier(newIcon, "drawable", Context.PackageName);
            fab.SetImageResource(resourceId);
        }

        private void FabOnClick (object sender, EventArgs eventArgs)
        {
            if (currentButton.Command.CanExecute (null))
            {
                currentButton.Command.Execute (this);
            }
        }

    }
}