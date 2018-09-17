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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfilePictureScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;
        private readonly ApplicationResourcesProvider resourceProvider = IoCManager.Resolve<ApplicationResourcesProvider>();
        private readonly ProfilePictureApiAccess client;
        private readonly ProfilePictureManager manager;

        private const string AdventurerImage = "ic_adventurer.png";
        private const string ProfessorImage = "ic_professor.png";
        private const int AvatarPixelHeight = 512;
        private const int AvatarPixelWidth = 512;

        public ProfilePictureScreenViewModel(MainPageViewModel mainPageViewModel)
        {
            this.mainPageViewModel = mainPageViewModel;

            KeepAvatarCommand = new Command(KeepAvatar);
            SaveNewAvatarCommand = new Command(SaveNewAvatar);
            ImagePickerCommand = new Command(PickImageAsync);
            ImageTappedCommand = new Command<int>(ImageTapped);

            //For access to the Userstore
            client = new ProfilePictureApiAccess(new UserApiClient(ServerEndpoints.RegisterUrl));
            manager = new ProfilePictureManager(client);

            PickImageEnabled = true;

            Avatar = Settings.AdventurerMode ? ImageSource.FromFile(AdventurerImage) : ImageSource.FromFile(ProfessorImage);

            PredAvatarGridBuilt = true;

        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            PredAvatarGridBuilt = true;

            ErrorMessageColor = "Black";
            ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";

            ChosenAvatarBytes = null;
            ChosenAvatarId = null;

            AvatarPreview = ImageSource.FromFile("predefined_avatar_empty");

            

            if (Settings.ProfilePicture == null)
            {
                var currentAvatar = await manager.GetProfilePicture(Settings.UserId, Settings.AccessToken);
                if (currentAvatar != null)
                {
                    Settings.ProfilePicture = Convert.ToBase64String(currentAvatar.Data);
                    Avatar = ImageSource.FromStream(() => new MemoryStream(currentAvatar.Data));
                }
            }
            else
            {
                var currentAvatarBytes = Convert.FromBase64String(Settings.ProfilePicture);
                Avatar = ImageSource.FromStream(() => new MemoryStream(currentAvatarBytes));
            }

            OnPropertyChanged();
            
            PredAvatarList();
            
        }

        public void KeepAvatar()
        {
            mainPageViewModel.SwitchToProfileView();
        }

        public async void SaveNewAvatar()
        {
            var result = new HttpResponseMessage();

            if (ChosenAvatarBytes != null && ChosenAvatarId == null)
            {
                ErrorMessageColor = "Black";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";

               
                result = await manager.PostProfilePicture(ChosenAvatarBytes, Settings.UserId, Settings.AccessToken);
            }
            else if (ChosenAvatarBytes == null && ChosenAvatarId != null)
            {
                ErrorMessageColor = "Black";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";

                result = await manager.PostPredProfilePicture(ChosenAvatarId, Settings.UserId, Settings.AccessToken);
            }
            else
            {
                ErrorMessageColor = "Red";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Selection}";
                return;
            }

            if (result != null)
            {
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    //Get uploaded picture to store it in the settings
                    var profilePicture = await manager.GetProfilePicture(Settings.UserId, Settings.AccessToken);

                    if (profilePicture != null)
                    {
                        Settings.ProfilePicture = Convert.ToBase64String(profilePicture.Data);
                        ChosenAvatarBytes = null;
                        ChosenAvatarId = null;
                        mainPageViewModel.SwitchToProfileView();
                    }
                    else
                    {
                        ErrorMessageColor = "Red";
                        ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Upload}";
                    }
                }
                else
                {
                    ErrorMessageColor = "Red";
                    ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Upload}";
                }
            }

            else
            {
                ErrorMessageColor = "Red";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Upload}";
            }
        }

        public async void PickImageAsync()
        {
            PickImageEnabled = false;

            var imageStream = await DependencyService.Get<IImagePicker>().GetImageStreamAsync();

            if (imageStream != null)
            {

                var memStream = new MemoryStream();
                await imageStream.CopyToAsync(memStream);
                ChosenAvatarBytes = memStream.ToArray();
                ChosenAvatarId = null;
                imageStream = (Stream)new MemoryStream(ChosenAvatarBytes, 0, ChosenAvatarBytes.Length);
                AvatarPreview = ImageSource.FromStream(() => imageStream);
                ErrorMessageColor = "Black";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";
            }

            PickImageEnabled = true;
        }

        public void ImageTapped(int image)
        {
            AvatarPreview = PredAvatars[image].ImageFull;
            ChosenAvatarBytes = null;
            ChosenAvatarId = PredAvatars[image].Id;
            for (var i = 0; i < HighlightColors.Length; i++)
            {
                HighlightColors[i] = Color.White;
            }
            HighlightColors[image] = resourceProvider.TryGetResourceColorvalue("PrimaryColor"); ;
        }

        private async void PredAvatarList()
        {
            PredAvatars = await manager.GetPredefinedProfilePictures(Settings.AccessToken);
            HighlightColors = new Color[PredAvatars.Length];

            for (var i = 0; i < PredAvatars.Length; i++)
            {
                HighlightColors[i] = Color.White;
            }

            foreach (var avatar in PredAvatars)
            {
                avatar.ImageSmallBytes = DependencyService.Get<IAvatarImageResizer>().ResizeAvatar(avatar.ImageFullBytes, AvatarPixelWidth, AvatarPixelHeight);
            }
            PredAvatarGridBuilt = false;

            MessagingCenter.Send<ProfilePictureScreenViewModel>(this, "PredAvatarList");
        }

        public ICommand KeepAvatarCommand { get; }
        public ICommand SaveNewAvatarCommand { get; }
        public ICommand ImagePickerCommand { get; }
        public ICommand ImageTappedCommand { get; }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        private string errorMessageColor;
        public string ErrorMessageColor
        {
            get { return errorMessageColor; }
            set { errorMessageColor = value; OnPropertyChanged(); }
        }

        private bool predAvatarGridBuilt;
        public bool PredAvatarGridBuilt
        {
            get { return predAvatarGridBuilt; }
            set { predAvatarGridBuilt = value; OnPropertyChanged(); }
        }

        public bool PickImageEnabled { get; private set; }

        private ImageSource avatar;
        public ImageSource Avatar
        {
            get { return avatar; }
            set { avatar = value; OnPropertyChanged(); }
        }

        private ImageSource avatarPreview;
        public ImageSource AvatarPreview
        {
            get { return avatarPreview; }
            set { avatarPreview = value; OnPropertyChanged(); }
        }

        public byte[] ChosenAvatarBytes { get; private set; }

        public string ChosenAvatarId { get; private set; }

        public PredProfilePicture[] PredAvatars { get; set; }

        private Color[] highlightColors;
        public Color[] HighlightColors
        {
            get { return highlightColors; }
            set { highlightColors = value; OnPropertyChanged(); }
        }

    }
}
