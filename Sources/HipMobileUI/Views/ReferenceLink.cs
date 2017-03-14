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
using System.Collections.Generic;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using Xamarin.Forms;

namespace HipMobileUI.Views
{
    public class ReferenceLink : Label
    {
        public static readonly BindableProperty SourcesProperty = BindableProperty.Create(nameof(Sources), typeof(List<Source>), typeof(ReferenceLink), defaultValue: new List<Source>());
        public static readonly BindableProperty ActionProperty = BindableProperty.Create(nameof(Action), typeof(Func<IInteractiveSourceAction>), typeof(ReferenceLink), defaultValue: null);

        public List<Source> Sources
        {
            get { return (List<Source>)GetValue(SourcesProperty); }
            set { SetValue(SourcesProperty, value); }
        }

        public Func<IInteractiveSourceAction> Action
        {
            get { return (Func<IInteractiveSourceAction>)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public string HtmlText
        {
            get
            {
                string text = Text.Replace("\n", "<br>");
                return text;
            }
        }
    }
}