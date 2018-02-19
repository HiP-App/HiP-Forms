// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers
{
    /// <summary>
    /// MarkupExtension which allows getting Texts from Strings.resx in Xaml
    /// If localization is required in the future, see https://developer.xamarin.com/guides/xamarin-forms/advanced/localization/
    /// on how to extend this class
    /// </summary>
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private readonly CultureInfo ci;
        private const string ResourceId = "PaderbornUniversity.SILab.Hip.Mobile.UI.Resources.Strings";

        public string Text { get; set; }

        public TranslateExtension()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
        }

        /// <summary>
        /// Returns the string value for the given Key in the Text property
        /// Returns the key itself, if the key is not present in the resource file
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            var translatedText = resourceManager.GetString(Text, ci);
            if (translatedText == null)
                Debug.WriteLine("Cannot find the resource: " + Text);
            return translatedText ?? Text;
        }
    }
}