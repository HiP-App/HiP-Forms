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
using System.Windows.Input;
using Xamarin.Forms;

namespace HipMobileUI.Behaviors
{
    /// <summary>
    /// Tapped behaviour for a list view which executes a bindable command once an element is tapped
    /// </summary>
    public class ListViewTappedItemBehavior : Behavior<ListView>
    {

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ListViewTappedItemBehavior));
        public static readonly BindableProperty InputConverterProperty = BindableProperty.Create(nameof(Converter), typeof(IValueConverter), typeof(ListViewTappedItemBehavior));

        /// <summary>
        /// Command to be executed if a user taps an list view element
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Converter needed to convert the tapped event into the view model of the tapped element
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(InputConverterProperty); }
            set { SetValue(InputConverterProperty, value); }
        }

        private ListView AssociatedObject { get; set; }

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.ItemTapped += OnListViewItemTapped;
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            bindable.ItemTapped -= OnListViewItemTapped;
            AssociatedObject = null;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        internal void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Command == null || e == null)
            {
                return;
            }

            var listView = sender as ListView;
            if (listView != null)
            {
                listView.SelectedItem = null;
            }

            object parameter = Converter.Convert(e, typeof(object), null, null);
            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

    }
}