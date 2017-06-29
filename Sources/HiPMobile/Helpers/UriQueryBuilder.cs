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
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers {
    public class UriQueryBuilder {

        public static string GetAdditionalParametersQuery (DateTimeOffset? timestamp, IList<int> includeOnly)
        {
            Dictionary<string, string> timestampParameter = new Dictionary<string, string> ();
            string includeOnlyString = "";
            if (timestamp.HasValue)
            {
                timestampParameter.Add ("timestamp", JsonConvert.SerializeObject (timestamp).Replace ("\"", ""));
            }
            else
            {
                includeOnlyString = includeOnlyString + "?";
            }
            if (includeOnly != null && includeOnly.Any ())
            {
                includeOnlyString = includeOnlyString + "&";
                foreach (int include in includeOnly)
                {
                    includeOnlyString = includeOnlyString + "IncludeOnly"  + "=" + include + "&";
                }
            }
            if (includeOnlyString.Length > 0)
            {
                includeOnlyString = includeOnlyString.Remove(includeOnlyString.Length - 1);
            }
            return ToQueryString (timestampParameter) + includeOnlyString;
        }

        public static string ToQueryString (Dictionary<string, string> parameters)
        {
            if (!parameters.Any ())
            {
                return string.Empty;
            }

            IEnumerable<string> segments = from key in parameters.Keys
                                           select $"{key}={Uri.EscapeDataString(parameters[key])}";
            return "?" + string.Join ("&", segments);
        }

    }
}