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

using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Container
{
    /// <summary>
    /// Extension of the <see cref="StackLayout"/> to enable displaying a stack layout containing a dynamically defined collection of children
    /// <example> 
    /// This generic sample shows how to use the layout. Refer to RouteOverviewView.xaml 
    /// for a concrete example. Important: the converter has to return <code><![CDATA[ObservableCollection<View>]]></code>
    /// so that the binding to the <see cref="ChildElements"/> property is working (and no List of subtypes of View).
    /// <code><![CDATA[ 
    ///   <container:BindableChildrenStackLayout  ChildElements="{Binding MyViewModelList, Converter= {StaticResource MyViewModelListToViewListConverter}}"/>
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    public class BindableChildrenStackLayout : StackLayout
    {
        public static readonly BindableProperty ChildElementsProperty = BindableProperty.Create(nameof(ChildElements), typeof(ObservableCollection<View>), typeof(BindableChildrenStackLayout), propertyChanged: ChildElementsPropertyChanged);

        /// <summary>
        /// Should be bound to the property of the viewmodel providing the data for the views
        /// so that the value converter can create views for this data.
        /// </summary>
        public ObservableCollection<View> ChildElements
        {
            get { return (ObservableCollection<View>)GetValue(ChildElementsProperty); }
            set { SetValue(ChildElementsProperty, value); }
        }

        /// <summary>
        /// Called if the property <see cref="ChildElements"/> is set. Adds the views as children to the stacklayout
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private static void ChildElementsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var layout = bindable as BindableChildrenStackLayout;
            var newList = newValue as ObservableCollection<View>;
            if (newList != null && layout != null)
            {
                layout.Children.Clear ();
                foreach (var view in newList)
                {
                    layout.Children.Add (view);
                }
            }
        }
    }
}