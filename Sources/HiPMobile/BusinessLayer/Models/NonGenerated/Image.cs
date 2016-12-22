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

using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using Realms;

namespace de.upb.hip.mobile.pcl.BusinessLayer.Models {
    public partial class Image {

        private readonly IImageDimension imgDimension = IoCManager.Resolve<IImageDimension>();

        private int _width { get; set; }
        [Ignored]
        public int Width {
            get {
                if (_width != 0)
                {
                    return _width;
                }
                else
                {
                    var w = imgDimension.GetDimensions (this) [0];
                    using (DbManager.StartTransaction ())
                    {
                        this._width = w;
                    }
                    return w;
                }
            }
            private set { this._width = value; }
        }

        private int _height { get; set; }
        [Ignored]
        public int Height
        {
            get
            {
                if (_height != 0)
                {
                    return _height;
                }
                else
                {
                    var h = imgDimension.GetDimensions(this)[1];
                    using (DbManager.StartTransaction ())
                    {
                        this._height = h;
                    }
                    return h;
                }
            }
            private set { this._height = value; }
        }

    }
}