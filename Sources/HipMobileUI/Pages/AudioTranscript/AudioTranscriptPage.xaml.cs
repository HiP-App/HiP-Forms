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

using System.ComponentModel;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using HipMobileUI.Navigation;
using HipMobileUI.ViewModels.Pages.AudioTranscript;
using Xamarin.Forms;

namespace HipMobileUI.Pages.AudioTranscript
{
    public delegate void CurrentPageChangedEventHandler();
    public partial class AudioTranscriptPage : TabbedPage, IViewFor<AudioTranscriptViewModel>
    {
        public new event CurrentPageChangedEventHandler CurrentPageChanged;

        public AudioTranscriptPage()
        {
            InitializeComponent ();
            PropertyChanged += OnPropertyChanged;
        }

        // Used to automatically switch tab page und scroll to reference
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPage")
            {
                RaiseCurrentPageChanged();
            }
        }

        private void RaiseCurrentPageChanged()
        {
            CurrentPageChanged?.Invoke();
        }

        public static readonly BindableProperty ActionProperty =
            BindableProperty.Create("Action", typeof(IInteractiveSourceAction), typeof(AudioTranscriptPage), defaultValue:null);

        public IInteractiveSourceAction Action
        {
            get { return (IInteractiveSourceAction)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        protected override void OnBindingContextChanged ()
        {
            base.OnBindingContextChanged();
            this.SetBinding(ActionProperty, "Action", BindingMode.TwoWay);
            Action = new SwitchTabAndScrollToSourceAction(this);
        }
    }
}
