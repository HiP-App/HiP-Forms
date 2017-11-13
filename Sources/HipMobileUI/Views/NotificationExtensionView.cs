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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    public class NotificationExtensionView : ContentView
    {
        private AbsoluteLayout layout;
        private AchievementNotificationView achievementNotificationView;
        
        private void CreateContent()
        {
            layout = new AbsoluteLayout();
            achievementNotificationView = new AchievementNotificationView
            {
                BindingContext = IoCManager.Resolve<AchievementNotificationViewModel>()
            };

            layout.Children.Add(ContentTemplate, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            layout.Children.Add(achievementNotificationView, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            Content = layout;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Make sure everything is initialized and create the content
            if (ContentTemplate != null && propertyName == "ContentTemplate")
                CreateContent();
        }

        public static readonly BindableProperty ContentTemplateProperty =
            BindableProperty.Create("ContentTemplate", typeof(View), typeof(View));
        public View ContentTemplate
        {
            get { return (View)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }
    }
}