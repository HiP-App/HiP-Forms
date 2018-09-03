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
// limitations under the License.using System;

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Map
{
    public class OsmMap : View
    {
        private Exhibit selectedExhibit;

        public OsmMap()
        {
            CenterCommand = new Command(() => { CenterLocationCalled?.Invoke((GeoLocation)CenterCommandParameter); });
        }

        public static readonly BindableProperty ExhibitSetProperty =
            BindableProperty.Create(nameof(ExhibitSet), typeof(IReadOnlyList<Exhibit>), typeof(OsmMap), null, propertyChanged: ExhibitPropertyChanged);

        public static readonly BindableProperty SelectedExhibitProperty= 
            BindableProperty.Create(nameof(SelectedExhibit), typeof(Exhibit), typeof(OsmMap), null, propertyChanged:ExhibitPropertyChanged);
        public static readonly BindableProperty GpsLocationProperty =
            BindableProperty.Create(nameof(GpsLocation), typeof(GeoLocation?), typeof(OsmMap), null, propertyChanged: GpsLocationPropertyChanged);

        public static readonly BindableProperty DetailsRouteProperty =
            BindableProperty.Create(nameof(DetailsRoute), typeof(Route), typeof(OsmMap), null, propertyChanged: DetailsRoutePropertyChanged);

        //Set this to true if you want to have direct polyline in routedetails screen for example
        public static readonly BindableProperty ShowDetailsRouteProperty =
            BindableProperty.Create(nameof(ShowDetailsRoute), typeof(bool), typeof(OsmMap), false);

        //Set this to true if want to have the navigation
        public static readonly BindableProperty ShowNavigationProperty = 
            BindableProperty.Create(nameof(ShowNavigation), typeof(bool), typeof(OsmMap), false);

        public static BindableProperty CenterCommandProperty =
            BindableProperty.Create(nameof(CenterCommand), typeof(ICommand), typeof(OsmMap), null, BindingMode.OneWayToSource);

        public static BindableProperty CenterCommandParameterProperty =
            BindableProperty.Create(nameof(CenterCommand), typeof(object), typeof(OsmMap), null);

        public ICommand CenterCommand
        {
            get => (ICommand)GetValue(CenterCommandProperty);
            set => SetValue(CenterCommandProperty, value);
        }

        public object CenterCommandParameter
        {
            get => GetValue(CenterCommandParameterProperty);
            set => SetValue(CenterCommandParameterProperty, value);
        }


        // Property accessor
        public IReadOnlyList<Exhibit> ExhibitSet
        {
            get => (IReadOnlyList<Exhibit>)GetValue(ExhibitSetProperty);
            set => SetValue(ExhibitSetProperty, value);
        }

        public Exhibit SelectedExhibit
        {
            get => (Exhibit)GetValue(SelectedExhibitProperty);
            set => SetValue(SelectedExhibitProperty, value);
        }
        public GeoLocation? GpsLocation
        {
            get => (GeoLocation?)GetValue(GpsLocationProperty);
            set => SetValue(GpsLocationProperty, value);
        }

        public Route DetailsRoute
        {
            get => (Route)GetValue(DetailsRouteProperty);
            set => SetValue(DetailsRouteProperty, value);
        }

        public bool ShowDetailsRoute
        {
            get => (bool)GetValue(ShowDetailsRouteProperty);
            set => SetValue(ShowDetailsRouteProperty, value);
        }

        public bool ShowNavigation
        {
            get => (bool)GetValue(ShowNavigationProperty);
            set => SetValue(ShowNavigationProperty, value);
        }

        private static void ExhibitPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!Equals(oldValue, newValue))
            {
                // inform all listeners that the ExhibitSet changed
                var map = (OsmMap)bindable;
                map.ExhibitSetChanged?.Invoke(map.ExhibitSet);
            }
        }

        private static void GpsLocationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!Equals(oldValue, newValue))
            {
                var map = (OsmMap)bindable;
                map.GpsLocationChanged?.Invoke(map.GpsLocation);
            }
        }

        private static void DetailsRoutePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!Equals(oldValue, newValue))
            {
                var map = (OsmMap)bindable;
                map.DetailsRouteChanged?.Invoke(map.DetailsRoute);
            }
        }

        public delegate void ExhibitSetChangedHandler(IReadOnlyList<Exhibit> set);

        public event ExhibitSetChangedHandler ExhibitSetChanged;

        public delegate void GpslocationChangedHandler(GeoLocation? location);

        public event GpslocationChangedHandler GpsLocationChanged;

        public delegate void DetailsRouteChangedHandler(Route route);

        public event DetailsRouteChangedHandler DetailsRouteChanged;

        public event GpslocationChangedHandler CenterLocationCalled;
    }
}