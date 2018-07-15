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
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserApiFetchers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfilePictureScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;

        public ICommand KeepAvatarCommand { get; }
        public ICommand SaveNewAvatarCommand { get; }
        public ICommand ImagePickerCommand { get; }

        private ProfilePictureApiAccess client;
        private ProfilePictureFetcher fetcher;

        private const string AdventurerImage = "ic_adventurer.png";
        private const string ProfessorImage = "ic_professor.png";

        public bool PickImageEnabled { get; private set; }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        //Attribute to choose an avatar from the list
        private PredefinedProfilePicture chosenAvatar;
        public PredefinedProfilePicture ChosenAvatar
        {
            get { return chosenAvatar; }
            set
            {
                chosenAvatar = value;
                if (chosenAvatar == null)
                    return;

                ChooseAvatar(chosenAvatar);

                ChosenAvatar = null;
            }
        }

        //Images for the display of the current avatar and the preview of the chosen avatar
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

        public Byte[] ChosenAvatarBytes { get; private set; }


        public ProfilePictureScreenViewModel(MainPageViewModel mainPageViewModel)
        {
            this.mainPageViewModel = mainPageViewModel;

            KeepAvatarCommand = new Command(KeepAvatar);
            SaveNewAvatarCommand = new Command(SaveNewAvatar);
            ImagePickerCommand = new Command(PickImageAsync);

            //For access to the Userstore
            client = new ProfilePictureApiAccess(new UserApiClient(ServerEndpoints.RegisterUrl));
            fetcher = new ProfilePictureFetcher(client);

            PickImageEnabled = true;

        }

        public void KeepAvatar()
        {
            ChosenAvatarBytes = null;
            mainPageViewModel.SwitchToProfileView();
        }

        public async void SaveNewAvatar()
        {
            if (ChosenAvatarBytes != null)
            {
                ErrorMessage = "";

                var imageStream = (Stream)new MemoryStream(ChosenAvatarBytes, 0, ChosenAvatarBytes.Length);
                var result = await client.PostProfilePicture(imageStream, Settings.UserId, Settings.AccessToken);

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    //Get uploaded picture to store it in the settings
                    var profilePicture = await fetcher.GetProfilePicture(Settings.UserId, Settings.AccessToken);

                    if (profilePicture != null)
                    {
                        Settings.ProfilePicture = Convert.ToBase64String(profilePicture.Data);
                        ChosenAvatarBytes = null;
                        mainPageViewModel.SwitchToProfileView();
                    }
                    else
                    {
                        ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Upload}";
                    }
                }
                else
                {
                    ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Upload}";
                }

                
                
            }
            else
            {
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Selection}";
            }

        }

        public void ChooseAvatar(PredefinedProfilePicture choseAvatar)
        {
            AvatarPreview = choseAvatar.Image;
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
                imageStream = (Stream) new MemoryStream(ChosenAvatarBytes, 0, ChosenAvatarBytes.Length);
                AvatarPreview = ImageSource.FromStream(() => imageStream);
            }

            PickImageEnabled = true;
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            ErrorMessage = "";

            AvatarPreview = ImageSource.FromFile("predefined_avatar_empty");

            if (Settings.ProfilePicture == null)
            {
                var currentAvatar = await fetcher.GetProfilePicture(Settings.UserId, Settings.AccessToken);
                if (currentAvatar != null)
                {
                    Settings.ProfilePicture = Convert.ToBase64String(currentAvatar.Data);
                    Avatar = ImageSource.FromStream(() => new MemoryStream(currentAvatar.Data));
                    return;
                }
            }
            else
            {
                var currentAvatarBytes = Convert.FromBase64String(Settings.ProfilePicture);
                Avatar = ImageSource.FromStream(() => new MemoryStream(currentAvatarBytes));
                return;
            }

            Avatar = Settings.AdventurerMode ? ImageSource.FromFile(AdventurerImage) : ImageSource.FromFile(ProfessorImage);
            OnPropertyChanged();
        }

        public ObservableCollection<PredefinedProfilePicture> Avatars { get; set; }

    }

    public class PredefinedProfilePicture
    {
        public ImageSource Image { get; set; }

        public string Title { get; set; }

        public PredefinedProfilePicture(string path, string title)
        {
            Image = ImageSource.FromFile(path);
            Title = title;
        }
    }
}
