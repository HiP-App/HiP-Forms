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

using System;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common.Contracts;
using Foundation;
using UIKit;

namespace HiPMobile.iOS {
    public class IosImageDimensions : IImageDimension{

        public int[] GetDimensions (Image img)
        {
            UIImage image = UIImage.LoadFromData(NSData.FromArray(img.Data));
            return new[] { Convert.ToInt32(image.Size.Width), Convert.ToInt32(image.Size.Height) };
        }

    }
}