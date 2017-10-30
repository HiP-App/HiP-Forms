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
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Pages.AudioTranscript
{
    public class SwitchTabAndScrollToSourceAction : IInteractiveSourceAction
    {
        public TabbedPage TabbedPage { get; set; }
        public Source Source { get; set; }

        public SwitchTabAndScrollToSourceAction(TabbedPage tabbedPage)
        {
            TabbedPage = tabbedPage;
        }

        private async void OnCurrentPageChanged(object sender, EventArgs e)
        {
            await Task.Delay(100);

            var sourcesPage = TabbedPage.Children[1] as SourcesPage;
            if (sourcesPage != null)
            {
                var sourcesListView = sourcesPage.Content as ListView;

                if (sourcesListView != null)
                {
                    // Scroll automatically to the tapped reference
                    ScrollToWithDelay(sourcesListView, Source, ScrollToPosition.Start);
                }
            }
            TabbedPage.CurrentPageChanged -= OnCurrentPageChanged;
        }

        public void Display(Source src)
        {
            var sourcesPage = TabbedPage.Children[1] as SourcesPage;
            if (sourcesPage != null)
            {
                Source = src;
                TabbedPage.CurrentPageChanged += OnCurrentPageChanged;

                // Switch to the reference tab
                TabbedPage.CurrentPage = sourcesPage;
            }
        }

        public static void ScrollToWithDelay(ListView lv, object item, ScrollToPosition position, bool animated = true)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                lv.ScrollTo(item, position, animated);
                return false;
            });
        }
    }
}