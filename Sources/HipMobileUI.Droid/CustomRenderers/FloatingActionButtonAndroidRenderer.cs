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
                fab.Click += FabOnClick;
                SetNativeControl(fab);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                e.OldElement.NormalColorChanged -= SetNormalColor;
                e.OldElement.RippleColorChanged -= SetRippleColor;
                e.OldElement.IconChanged -= SetIcon;
            }
            if (e.NewElement != null)
            {
                // set init values
                currentButton = e.NewElement;
                SetIcon(currentButton.Icon);
                SetNormalColor (currentButton.NormalColor);
                SetRippleColor (currentButton.RippleColor);

                // Subscribe
                currentButton.NormalColorChanged+=SetNormalColor;
                currentButton.RippleColorChanged+=SetRippleColor;
                currentButton.IconChanged+=SetIcon;
            }
        }

        private void SetRippleColor (Color newColor)
        {
            fab.SetRippleColor (newColor.ToAndroid());
        }

        private void SetNormalColor (Color newColor)
        {
            fab.SetBackgroundColor(newColor.ToAndroid());
        }


        private void SetIcon (string newIcon)
        {
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