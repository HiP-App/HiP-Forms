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

using System.IO;
using Android.Content.Res;
using de.upb.hip.mobile.pcl.Common.Contracts;

namespace de.upb.hip.mobile.droid.Contracts {
    public class AndroidDataLoader : IDataLoader {

        private readonly AssetManager manager;

        public AndroidDataLoader (AssetManager manager)
        {
            this.manager = manager;
        }

        public byte[] LoadByteData (string name)
        {
            var buffer = new byte[16 * 1024];
            byte[] data;
            using (var input = manager.Open(name))
            {
                using (var ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    data = ms.ToArray();
                }
            }
            return data;
        }

    }
}