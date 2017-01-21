using HipMobileUI.iOS;
using HipMobileUI.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Link), typeof(LinkRenderer))]
namespace HipMobileUI.iOS
{
    public class LinkRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> elementChangedEventArgs)
        {
            base.OnElementChanged(elementChangedEventArgs);

            if (Control != null)
            {
                //TODO
                // do whatever you want to the Label here!
                Control.BackgroundColor = UIColor.FromRGB(204, 153, 255);
                Control.TextColor = UIColor.Green;
            }
        }
    }
}