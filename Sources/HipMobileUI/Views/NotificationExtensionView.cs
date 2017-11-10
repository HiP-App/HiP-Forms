using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    public class NotificationExtensionView : ContentView
    {
        private AbsoluteLayout fusionContent;
        private AchievementNotificationView achievementNotificationView;
        
        private void CreateContent()
        {
            fusionContent = new AbsoluteLayout();
            achievementNotificationView = new AchievementNotificationView
            {
                BindingContext = AchievementNotificationViewModel
            };

            fusionContent.Children.Add(ContentTemplate, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            fusionContent.Children.Add(achievementNotificationView, new Rectangle(0, 0, 1, 0.2), AbsoluteLayoutFlags.All);

            Content = fusionContent;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Make sure everything is initialized and create the content
            if (ContentTemplate != null && AchievementNotificationViewModel != null && propertyName == "AchievementNotificationViewModel")
                CreateContent();
        }

        public static readonly BindableProperty ContentTemplateProperty =
            BindableProperty.Create("ContentTemplate", typeof(View), typeof(View));
        public View ContentTemplate
        {
            get { return (View)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly BindableProperty AchievementNotificationViewModelProperty =
            BindableProperty.Create("AchievementNotificationViewModel", typeof(AchievementNotificationViewModel), typeof(AchievementNotificationViewModel));
        public AchievementNotificationViewModel AchievementNotificationViewModel
        {
            get { return (AchievementNotificationViewModel)GetValue(AchievementNotificationViewModelProperty); }
            set { SetValue(AchievementNotificationViewModelProperty, value); }
        }
    }
}