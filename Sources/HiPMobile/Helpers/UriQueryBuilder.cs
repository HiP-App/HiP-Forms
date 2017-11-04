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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers
{
    public class UriQueryBuilder
    {
        public static string GetAdditionalParametersQuery(DateTimeOffset? timestamp, IList<int> includeOnly)
        {
            var keys = new List<string>();
            var values = new List<string>();
            if (timestamp.HasValue)
            {
                keys.Add("timestamp");
                values.Add(JsonConvert.SerializeObject(timestamp).Replace("\"", ""));
            }
            if (includeOnly != null && includeOnly.Any())
            {
                foreach (int include in includeOnly)
                {
                    keys.Add("includeOnly");
                    values.Add(include.ToString());
                }
            }

            return ToQueryString(keys, values);
        }

        public static string ToQueryString(List<string> keys, List<string> values)
        {
            if (!keys.Any())
            {
                return string.Empty;
            }

            var segments = new List<string>();

            for (int i = 0; i < keys.Count; i++)
            {
                segments.Add($"{keys[i]}={Uri.EscapeDataString(values[i])}");
            }

            return "?" + string.Join("&", segments);
        }
    }
}