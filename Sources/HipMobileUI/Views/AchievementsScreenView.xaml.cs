﻿using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;
using Xamarin.Forms.Xaml;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AchievementsScreenView : IViewFor<AchievementsScreenViewModel>
    {
        public AchievementsScreenView()
        {
            InitializeComponent();
        }
    }
}