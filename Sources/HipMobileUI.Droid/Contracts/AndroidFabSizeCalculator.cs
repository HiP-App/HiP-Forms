using Android.Content.Res;
using Android.OS;
using Android.Util;
using de.upb.hip.mobile.pcl.Common.Contracts;

namespace de.upb.hip.mobile.droid.Contracts
{
    class AndroidFabSizeCalculator : IFabSizeCalculator {

        private readonly float scale = Resources.System.DisplayMetrics.Density;

        public int CalculateFabSize ()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                return 56;
            }
            else
            {
                return (int)(56 * scale + 0.5);
            }
            
        }

    }
}