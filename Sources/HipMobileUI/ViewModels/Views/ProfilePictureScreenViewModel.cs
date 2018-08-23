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
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
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

        public ICommand ImageTappedCommand { get; }

        private ProfilePictureApiAccess client;
        private ProfilePictureFetcher fetcher;

        private const string AdventurerImage = "ic_adventurer.png";
        private const string ProfessorImage = "ic_professor.png";

        private bool predAvatarGridBuild;
        public bool PredAvatarGridBuilt
        {
            get { return predAvatarGridBuild; }
            set { predAvatarGridBuild = value; OnPropertyChanged(); }
        }

        public bool PickImageEnabled { get; private set; }

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

        public int PredAvatarCount;

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

        public byte[] ChosenAvatarBytes { get; private set; }

        public ProfilePictureScreenViewModel(MainPageViewModel mainPageViewModel)
        {
            this.mainPageViewModel = mainPageViewModel;

            KeepAvatarCommand = new Command(KeepAvatar);
            SaveNewAvatarCommand = new Command(SaveNewAvatar);
            ImagePickerCommand = new Command(PickImageAsync);
            ImageTappedCommand = new Command<int>(ImageTapped);

            //For access to the Userstore
            client = new ProfilePictureApiAccess(new UserApiClient(ServerEndpoints.RegisterUrl));
            fetcher = new ProfilePictureFetcher(client);

            PickImageEnabled = true;

            Avatar = Settings.AdventurerMode ? ImageSource.FromFile(AdventurerImage) : ImageSource.FromFile(ProfessorImage);

            MockPredAvatarList();
            ResizeAvatars();

            PredAvatarGridBuilt = false;

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
                ErrorMessageColor = "Black";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";

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
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_Error_Selection}";
            }

        }

        public void ChooseAvatar(PredefinedProfilePicture choseAvatar)
        {
            //AvatarPreview = choseAvatar.Image1;
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
                imageStream = (Stream)new MemoryStream(ChosenAvatarBytes, 0, ChosenAvatarBytes.Length);
                AvatarPreview = ImageSource.FromStream(() => imageStream);
                ErrorMessageColor = "Black";
                ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";
            }

            PickImageEnabled = true;
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            ErrorMessageColor = "Black";
            ErrorMessage = $"{Strings.ProfilePictureScreenViewModel_NoError}";

            AvatarPreview = ImageSource.FromFile("predefined_avatar_empty");

            PredAvatarGridBuilt = false;

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



            OnPropertyChanged();
        }

        public PredefinedProfilePicture[] Avatars { get; set; }

        private Color[] highlightColors;
        public Color[] HighlightColors
        {
            get { return highlightColors; }
            set { highlightColors = value; OnPropertyChanged(); }
        } //Wie kann ich die HighlightColor ändern, so dass die view die Änderung auch erkennt? OnPropertyChanged für das Array?

        public void ImageTapped(int image)
        {
            AvatarPreview = Avatars[image].ImageFull;
            for (var i = 0; i < HighlightColors.Length; i++)
            {
                HighlightColors[i] = Color.White;
                OnPropertyChanged("HighlightColors[i]");
            }
            HighlightColors[image] = Color.Blue;
            OnPropertyChanged("HighlightColors[image]");


            /*if (image.Equals("1"))
            {
                AvatarPreview = ChosenAvatar.Image1;
            }
            else if (image.Equals("2"))
            {
                AvatarPreview = ChosenAvatar.Image2;
            }
            else
            {
                AvatarPreview = ChosenAvatar.Image3;
            }*/

        }

        public void ResizeAvatars()
        {
            var avatarHeight = 512;
            var avatarWidth = 512;

            foreach (var avatar in Avatars)
            {
                var avatarSmall = DependencyService.Get<IAvatarImageResizer>().ResizeAvatar(avatar.ImageFullBytes, avatarWidth, avatarHeight);
                avatar.ImageSmallBytes = avatarSmall;
            }

        }

        private void MockPredAvatarList()
        {
            PredAvatarCount = 20;
            Avatars = new PredefinedProfilePicture[PredAvatarCount];
            HighlightColors = new Color[PredAvatarCount];

            var preds = new PredefinedProfilePictureStrings();
            var imageString = preds.PredefinedAvatarDog;
            var imageBytes = Convert.FromBase64String(imageString);

            for (var i = 0; i < Avatars.Length; i++)
            {
                
                Avatars[i] = new PredefinedProfilePicture(imageBytes, "picture");
                HighlightColors[i] = Color.White;
            }
        }

    }

    public class PredefinedProfilePicture
    {
        /*public ImageSource Image1 { get; set; }
        public ImageSource Image2 { get; set; }
        public ImageSource Image3 { get; set; }

        private string highlightColor1;
        public string HighlightColor1 {
            get { return highlightColor1; }
            set { highlightColor1 = value; }
        }
        private string highlightColor2;
        public string HighlightColor2 {
            get { return highlightColor2; }
            set { highlightColor2 = value; }
        }
        private string highlightColor3;
        public string HighlightColor3 {
            get { return highlightColor3; }
            set { highlightColor3 = value; }
        }

        public string Title { get; set; }

        public PredefinedProfilePicture(string path1, string path2, string path3, string title)
        {
            Image1 = ImageSource.FromFile(path1);
            Image2 = ImageSource.FromFile(path2);
            Image3 = ImageSource.FromFile(path3);
            HighlightColor1 = "White";
            HighlightColor2 = "White";
            HighlightColor3 = "White";
            Title = title;
        }*/

        public ImageSource ImageFull { get; set; }

        private byte[] imageFullBytes;
        public byte[] ImageFullBytes
        {
            get
            {
                return imageFullBytes;
            }
            set
            {
                imageFullBytes = value;
                ImageFull = ImageSource.FromStream(() => new MemoryStream(imageFullBytes));
            }
        }
        public ImageSource ImageSmall { get; set; }

        private byte[] imageSmallBytes;
        public byte[] ImageSmallBytes
        {
            get
            {
                return imageSmallBytes;
            }
            set
            {
                imageSmallBytes = value;
                ImageSmall = ImageSource.FromStream(() => new MemoryStream(imageSmallBytes));
            }
        }

        private string highlightColor;
        public string HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }

        public string Title { get; set; }

        public PredefinedProfilePicture(byte[] imageBytes, string title)
        {
            ImageFullBytes = imageBytes;
            HighlightColor = "White";
            Title = title;
        }
    }
}
