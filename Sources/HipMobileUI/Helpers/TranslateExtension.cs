using System;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HipMobileUI.Helpers
{
    /// <summary>
    /// MarkupExtension which allows getting Texts from Strings.resx in Xaml
    /// If localization is required in the future, see https://developer.xamarin.com/guides/xamarin-forms/advanced/localization/
    /// on how to extend this class
    /// </summary>
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension {

        private const string ResourceFile = "HipMobileUI.Resources.Strings";

        public string Text { get; set; }

        /// <summary>
        /// Returns the string value for the given Key in the Text property
        /// Returns the key itself, if the key is not present in the resource file
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public object ProvideValue (IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            ResourceManager resmgr = new ResourceManager(ResourceFile
                                , typeof(TranslateExtension).GetTypeInfo().Assembly);

            var translation = resmgr.GetString (Text) ?? Text; //If the given key does not exist, return the key itself

            return translation;
        }

    }
}
