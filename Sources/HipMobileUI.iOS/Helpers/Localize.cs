using System.Threading;
using System.Globalization;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(PaderbornUniversity.SILab.Hip.Mobile.iOS.Helpers.Localize))]

namespace PaderbornUniversity.SILab.Hip.Mobile.iOS.Helpers
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
            var netLanguage = "en";
            var test = NSLocale.PreferredLanguages;
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                netLanguage = NSLocale.PreferredLanguages[0].Replace("_", "-");
            }

            // this gets called a lot - try/catch can be expensive so consider caching or something
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