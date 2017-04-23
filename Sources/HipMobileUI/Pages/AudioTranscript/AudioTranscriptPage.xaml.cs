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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages.AudioTranscript;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages.AudioTranscript
{
    public delegate void CurrentPageChangedEventHandler();
    public partial class AudioTranscriptPage : TabbedPage, IViewFor<AudioTranscriptViewModel>
    {
        public new event CurrentPageChangedEventHandler CurrentPageChanged;

        private Page FormsPage => Application.Current.MainPage;

        private Xamarin.Forms.NavigationPage NavigationPage
        {
            get
            {
                var mainPage = FormsPage as MainPage;
                return mainPage?.Navigationpage;
            }
        }

        public AudioTranscriptPage()
        {
            InitializeComponent ();
            PropertyChanged += OnPropertyChanged;

            BarBackgroundColor = NavigationPage.BarBackgroundColor;
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

        void OnToolbarItemClicked(object sender, AsyncCompletedEventArgs args)
        {
            ContentPage subtitlesPage = this.FindByName<ContentPage>("SubtitlesPage");
            ContentPage sourcesPage = this.FindByName<ContentPage>("SourcesPage");

            switch (Settings.AudioTranscriptFontSize)
            {
                case 14:
                    Settings.AudioTranscriptFontSize = 19;
                    break;
                case 19:
                    Settings.AudioTranscriptFontSize = 24;
                    break;
                case 24:
                    Settings.AudioTranscriptFontSize = 14;
                    break;
            }

            ((SubtitleViewModel)subtitlesPage.BindingContext).FontSize = Settings.AudioTranscriptFontSize;
            ((SourcesViewModel)sourcesPage.BindingContext).FontSize = Settings.AudioTranscriptFontSize;
        }
    }
}
