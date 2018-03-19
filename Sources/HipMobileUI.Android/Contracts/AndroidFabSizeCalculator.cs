// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using Android.Content.Res;
using Android.OS;
using Android.Util;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Droid.Contracts
{
    class AndroidFabSizeCalculator : IFabSizeCalculator
    {
        private readonly float density = Resources.System.DisplayMetrics.Density;

        public int CalculateFabSize()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                // on lollipop and newer android uses dp as a unit of size
                return 56;
            }
            // compat libraries use pixel, therefore convert dp to pixel
            return (int) (56 * density);
        }

        public int GetOsVersionNumber()
        {
            return (int) Build.VERSION.SdkInt;
        }
    }
}