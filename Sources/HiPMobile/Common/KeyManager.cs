// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Common
{
    public class KeyManager
    {
        private static KeyManager instance;

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static KeyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KeyManager();
                }
                return instance;
            }
        }

        private readonly List<IKeyProvider> keyProvider;

        private KeyManager()
        {
            keyProvider = new List<IKeyProvider>();
        }

        /// <summary>
        /// Register a key provider which can be querried for keys.
        /// </summary>
        /// <param name="provider">The provider to register.</param>
        public void RegisterProvider(IKeyProvider provider)
        {
            if (provider != null)
            {
                keyProvider.Add(provider);
            }
        }

        /// <summary>
        /// Unregister a key provider so it is not querried for keys anymore.
        /// </summary>
        /// <param name="provider">The provider to unregister.</param>
        public void UnregisterProvider(IKeyProvider provider)
        {
            if (provider != null)
            {
                keyProvider.Remove(provider);
            }
        }

        /// <summary>
        /// Tries to get the key for the given name. If no key exists, an empty string is returned.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <returns>They key if it exists, an empty string otherwise.</returns>
        public string GetKey(string name)
        {
            foreach (var provider in keyProvider)
            {
                string key = provider.GetKeyByName(name);
                if (!string.IsNullOrEmpty(key))
                {
                    return key;
                }
            }
            return string.Empty;
        }
    }
}