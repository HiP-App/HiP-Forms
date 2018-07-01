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
using System.Windows.Input;
using Itinero.LocalGeo.Elevation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserApiFetchers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;


namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfilePictureScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;

        public ICommand KeepAvatarCommand { get; }
        public ICommand SaveNewAvatarCommand { get; }
        public ICommand ImagePickerCommand { get; }

        public bool PickImageEnabled { get; private set; }

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

        public Stream ChosenAvatarStream { get; private set; }


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
            mainPageViewModel.SwitchToProfileView();
        }

        public async void SaveNewAvatar()
        {
            await client.PostProfilePicture(ChosenAvatarStream, Settings.UserId, Settings.AccessToken);

            var profilePicture = await fetcher.GetProfilePicture(Settings.UserId, Settings.AccessToken);

            var pictures = new PredefinedProfilePictureStrings();
            var picture = Convert.FromBase64String(pictures.PredefinedAvatarDog);
            ImageSource av = ImageSource.FromStream(() => new MemoryStream(picture));
            //Avatar = profilePicture == null ? ImageSource.FromFile("ic_professor.png") : ImageSource.FromStream(() => new MemoryStream(profilePicture.Data));
            Avatar = profilePicture == null ? av : ImageSource.FromStream(() => new MemoryStream(profilePicture.Data));
            AvatarPreview = ImageSource.FromFile("predefined_avatar_empty");

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
                AvatarPreview = ImageSource.FromStream(() => imageStream);
                ChosenAvatarStream = imageStream;
            }

            PickImageEnabled = true;
            
        }




        private ProfilePictureApiAccess client;
        private ProfilePictureFetcher fetcher;

        

        public ObservableCollection<PredefinedProfilePicture> Avatars { get; set; }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            

            var pictures = new PredefinedProfilePictureStrings();
            var picture = Convert.FromBase64String(pictures.PredefinedAvatarDog);
            ImageSource av = ImageSource.FromStream(() => new MemoryStream(picture));



            //await client.PostProfilePicture(imageStream, userId);


            var profilePicture = await fetcher.GetProfilePicture(Settings.UserId, Settings.AccessToken);

            //Avatar = profilePicture == null ? ImageSource.FromFile("ic_professor.png") : ImageSource.FromStream(() => new MemoryStream(profilePicture.Data));
            Avatar = profilePicture == null ? av : ImageSource.FromStream(() => new MemoryStream(profilePicture.Data));
            AvatarPreview = ImageSource.FromFile("predefined_avatar_empty");


        }


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
