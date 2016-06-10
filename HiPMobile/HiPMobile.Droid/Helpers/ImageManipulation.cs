using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace de.upb.hip.mobile.droid.Helpers {
    class ImageManipulation {

        

        public static Bitmap getCroppedImage (Bitmap bitmap, int radius)
        {
            Bitmap scaledBitmap;
            if (bitmap.Width != radius || bitmap.Height != radius)
            {
                scaledBitmap = Bitmap.CreateScaledBitmap (bitmap, radius, radius, false);
            }
            else
            {
                scaledBitmap = bitmap;
            }
            Bitmap output = Bitmap.CreateBitmap (scaledBitmap.Width,
                                                 scaledBitmap.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas (output);

            Paint paint = new Paint ();
            Rect rect =  new Rect (0, 0, scaledBitmap.Width, scaledBitmap.Height);

            paint.AntiAlias = true;
            paint.FilterBitmap = true;
            paint.Dither = true;
            canvas.DrawARGB (0, 0, 0, 0);
            paint.Color =Color.ParseColor ("#BAB399");
            canvas.DrawCircle (scaledBitmap.Width  / 2 + 0.7f, scaledBitmap.Height / 2 + 0.7f,
                               scaledBitmap.Width  / 2 + 0.1f, paint);
            paint.SetXfermode (new PorterDuffXfermode (PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap (scaledBitmap, rect, rect, paint);

            return output;
        }

        /**
         * Converts a square image into a round image with marker for maps.
         *
         * @param bitmap  the rectangular image
         * @param context Android Context
         * @return image of marker with imput image inside
         */

        public static Bitmap getMarker (Bitmap bitmap, Context context)
        {
            bitmap = ImageManipulation.getCroppedImage (bitmap, 55);

            Bitmap markerBitmap = BitmapFactory.DecodeResource (
                context.Resources, Resource.Drawable.marker_blue);
            int markerBitmapWidth = markerBitmap.Width;
            int markerBitmapHeight = markerBitmap.Height;
            int bitmapWidth = bitmap.Width;

            float marginLeft = (float) (markerBitmapWidth * 0.5 - bitmapWidth * 0.5);
            float marginTop = (float) 13;

            Bitmap overlayBitmap = Bitmap.CreateBitmap (
                markerBitmapWidth, markerBitmapHeight, markerBitmap.GetConfig ());
            Canvas canvas = new Canvas (overlayBitmap);
            canvas.DrawBitmap (markerBitmap, new Matrix (), null);
            canvas.DrawBitmap (bitmap, marginLeft, marginTop, null);

            return overlayBitmap;
        }

    }
}