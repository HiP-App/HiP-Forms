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

using System;
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
    public class TranslateExtension : IMarkupExtension {

        private const string ResourceFile = "PaderbornUniversity.SILab.Hip.Mobile.UI.Resources.Strings";

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
