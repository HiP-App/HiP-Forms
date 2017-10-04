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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts
{
    public interface IImageDimension
    {
        /// <summary>
        /// Gets the dimensions of the given image.
        /// </summary>
        /// <param name="img">The image.</param>
        /// <returns>The dimensions formated as an array: [width, height]</returns>
        int[] GetDimensions(Image img);
    }
}