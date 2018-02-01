using System.Threading;
using System.Globalization;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using Xamarin.Forms;

[assembly: Dependency(typeof(PaderbornUniversity.SILab.Hip.Mobile.Droid.Helpers.Localize))]

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Helpers
{
    public class Localize : ILocalize
    {
        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = Java.Util.Locale.Default.ToString().Replace("_", "-");

            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException)
            {
                try
                {
                    ci = new CultureInfo(netLanguage.Split('-')[0]);
                }
                catch (CultureNotFoundException)
                {
                    // falling back to English
                    ci = new CultureInfo("en");
                }

            }

            return ci;
        }
    }
}
