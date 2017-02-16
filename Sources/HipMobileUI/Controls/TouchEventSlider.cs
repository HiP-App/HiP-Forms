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
using Xamarin.Forms;

namespace HipMobileUI.Controls
{
    /// <summary>
    /// This slider behaves like a normal slider but offers events that fire when a touch event starts and ends. Idea from https://forums.xamarin.com/discussion/88051/slider-control-as-audio-seekbar
    /// </summary>
    public class TouchEventSlider : Slider
    {
        // Events for external use (for example XAML)
        public event EventHandler TouchDown;
        public event EventHandler TouchUp;

        // Events called by renderers
        public readonly EventHandler TouchDownEvent;
        public readonly EventHandler TouchUpEvent;

        public TouchEventSlider()
        {
            TouchDownEvent = (sender, args) => TouchDown?.Invoke (this, args);
            TouchUpEvent = (sender, args) => TouchUp?.Invoke (this, args);
        }
    }

    /// <summary>
    /// Event args carrying a single double value;
    /// </summary>
    public class ValueEventArgs : EventArgs {

        public ValueEventArgs (double value)
        {
            Value = value;
        }
        public double Value { get; set; }

    }
}
