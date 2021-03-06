﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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

using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Controls
{
    /// <summary>
    /// Toolbar item that can be hidden.
    /// </summary>
    public class HideableToolbarItem : ToolbarItem
    {
        /// <summary>
        /// Indicates whether the toolbaritem is visible in the toolbar
        /// </summary>
        public bool IsVisible
        {
            get => (bool) GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(HideableToolbarItem), propertyChanged: OnIsVisibleChanged, defaultValue: false);

        private bool isInitialized;
        
        // Removing a ToolbarItem from a Toolbar resets the BindingContext of the item,
        // so we save it in here and will restore it before adding it back to the Toolbar
        private object savedContext;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (!isInitialized)
            {
                // Set initial visibility state
                OnIsVisibleChanged(this, false, IsVisible);
                isInitialized = true;
            }
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = bindable as HideableToolbarItem;

            var page = item?.Parent as ContentPage;
            var items = page?.ToolbarItems;

            if (items == null)
                return;

            var isVisible = (bool) newvalue;

            if (isVisible && !items.Contains(item))
            {
                if (item.savedContext != null)
                {
                    item.BindingContext = item.savedContext;
                }

                // Enqueue into event loop to avoid modifying items while it's iterated over by the internal Xamarin.Forms method
                // that eventually causes OnBindingContextChanged and then OnIsVisibleChanged to be called
                Device.BeginInvokeOnMainThread(() => items.Add(item));
            }
            else if (!isVisible && items.Contains(item))
            {
                item.savedContext = item.BindingContext;

                // Enqueue into event loop to avoid modifying items while it's iterated over by the internal Xamarin.Forms method
                // that eventually causes OnBindingContextChanged and then OnIsVisibleChanged to be called
                Device.BeginInvokeOnMainThread(() => items.Remove(item));
            }
        }
    }
}