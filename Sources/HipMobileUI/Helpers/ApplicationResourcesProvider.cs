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
using System.Collections.Generic;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers
{
    /// <summary>
    /// Should be initialized with Application.Current.Resources for the real application
    /// Used for Unit tests to mock Application.Current.Resources
    /// </summary>
    public class ApplicationResourcesProvider
    {

        //Constructor overloading to be avoided due to Unity throwing System.InvalidOperationException on multiple constructors of same length
        public ApplicationResourcesProvider(Dictionary<string, object> resources)
        {
            Resources = resources;
        }

        private Dictionary<string, object> Resources { get; }

        /// <summary>
        /// Gets the resource value for the given name
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns></returns>
        public object GetResourceValue(string resourceName)
        {
            return Resources[resourceName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldException">when <paramref name="fieldName"/> is not fount in the dictionary.</exception>
        /// <exception cref="UnhandledCastExceptionn">when it was not possible to cast value belonging to <paramref name="fieldName"/> to type Xamarin.Forms.Color. </exception>
        public Color TryGetResourceColorvalue(string fieldName)
        {
            if (Resources.ContainsKey(fieldName))
                return (Color)Resources[fieldName];
            else
                throw new MissingFieldException("Field " + fieldName + " not fount in dictionary " + Resources);
        }

        /// <summary>
        /// Changes the value of the dictionary entry with given key <paramref name="fieldName"/>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        /// <exception cref="MissingFieldException">when <paramref name="fieldName"/> is not fount in the dictionary.</exception>
        public void ChangeResourceValue(string fieldName, object newValue)
        {
            if (Resources.ContainsKey(fieldName))
                Resources[fieldName] = newValue;
            else
                throw new MissingFieldException("Field " + fieldName + " not fount in dictionary " + Resources);
        }
    }
}