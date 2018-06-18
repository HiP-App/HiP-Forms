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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Input;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserApiFetchers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    public class ProfilePictureScreenViewModel : NavigationViewModel
    {
        private readonly MainPageViewModel mainPageViewModel;

        public ICommand KeepAvatarCommand { get; }
        public ICommand SaveNewAvatarCommand { get; }

        //public ICommand ChosenAvatarCommand { get; }

        public ProfilePictureScreenViewModel(MainPageViewModel mainPageViewModel)
        {
            this.mainPageViewModel = mainPageViewModel;

            KeepAvatarCommand = new Command(KeepAvatar);
            SaveNewAvatarCommand = new Command(SaveNewAvatar);
            //ChosenAvatarCommand = new Command(ChooseAvatar);

            data = Convert.FromBase64String(@"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
4qCD/hc5p4Kfv6DqfgAAAABJRU5ErkJggg==");
            //ImageSource av = ImageSource.FromStream(() => new MemoryStream(data));

            client = new ProfilePictureApiAccess(new UserApiClient(ServerEndpoints.RegisterUrl));
            fetcher = new ProfilePictureFetcher(client);


            //Create Predefined Avatars for the View
            var avatar1 = new PredefinedProfilePicture(Avatar1, Avatar1Title);
            var avatar2 = new PredefinedProfilePicture(Avatar2, Avatar2Title);
            var avatar3 = new PredefinedProfilePicture(Avatar3, Avatar3Title);
            var avatar4 = new PredefinedProfilePicture(Avatar4, Avatar4Title);
            var avatar5 = new PredefinedProfilePicture(Avatar5, Avatar5Title);
            var avatar6 = new PredefinedProfilePicture(Avatar6, Avatar6Title);
            var avatar7 = new PredefinedProfilePicture(Avatar7, Avatar7Title);
            var avatar8 = new PredefinedProfilePicture(Avatar8, Avatar8Title);
            var avatar9 = new PredefinedProfilePicture(Avatar9, Avatar9Title);
            var avatar10 = new PredefinedProfilePicture(Avatar10, Avatar10Title);
            var avatar11 = new PredefinedProfilePicture(Avatar11, Avatar11Title);
            var avatar12 = new PredefinedProfilePicture(Avatar12, Avatar12Title);
            var avatar13 = new PredefinedProfilePicture(Avatar13, Avatar13Title);
            var avatar14 = new PredefinedProfilePicture(Avatar14, Avatar14Title);
            var avatar15 = new PredefinedProfilePicture(Avatar15, Avatar15Title);
            var avatar16 = new PredefinedProfilePicture(Avatar16, Avatar16Title);
            var avatar17 = new PredefinedProfilePicture(Avatar17, Avatar17Title);
            var avatar18 = new PredefinedProfilePicture(Avatar18, Avatar18Title);
            var avatar19 = new PredefinedProfilePicture(Avatar19, Avatar19Title);
            var avatar20 = new PredefinedProfilePicture(Avatar20, Avatar20Title);
            var avatar21 = new PredefinedProfilePicture(Avatar21, Avatar21Title);

            Avatars = new ObservableCollection<PredefinedProfilePicture>
            {
                avatar1, avatar2, avatar3, avatar4, avatar5, avatar6, avatar7,
                avatar8, avatar9, avatar10, avatar11, avatar12, avatar13, avatar14,
                avatar15, avatar16, avatar17, avatar18, avatar19, avatar20, avatar21
            };

        }

        public void KeepAvatar()
        {

        }

        public void SaveNewAvatar()
        {

        }

        public void ChooseAvatar(PredefinedProfilePicture chosenAvatar)
        {
            AvatarPreview = chosenAvatar.Image;
        }

        private PredefinedProfilePicture _chosenAvatar;
        public PredefinedProfilePicture ChosenAvatar
        {
            get { return _chosenAvatar; }
            set
            {
                _chosenAvatar = value;
                if (_chosenAvatar == null)
                    return;

               ChooseAvatar(_chosenAvatar);

                ChosenAvatar = null;
            }
        }
       

        private ProfilePictureApiAccess client;
        private ProfilePictureFetcher fetcher;

        //Placeholder
        private byte[] data;

        //Predefined Avatars
        private const string Avatar1 = "predefined_avatar_bear.png";
        private const string Avatar2 = "predefined_avatar_catlady2.png";
        private const string Avatar3 = "predefined_avatar_catpurr2.png";
        private const string Avatar4 = "predefined_avatar_chipmunk.png";
        private const string Avatar5 = "predefined_avatar_cuttlefish.png";
        private const string Avatar6 = "predefined_avatar_dog.png";
        private const string Avatar7 = "predefined_avatar_fox.png";
        private const string Avatar8 = "predefined_avatar_giraffe.png";
        private const string Avatar9 = "predefined_avatar_hedgehog.png";
        private const string Avatar10 = "predefined_avatar_honeybee.png";
        private const string Avatar11 = "predefined_avatar_koala.png";
        private const string Avatar12 = "predefined_avatar_ladybeetle.png";
        private const string Avatar13 = "predefined_avatar_lion.png";
        private const string Avatar14 = "predefined_avatar_owl.png";
        private const string Avatar15 = "predefined_avatar_panda.png";
        private const string Avatar16 = "predefined_avatar_penguin.png";
        private const string Avatar17 = "predefined_avatar_pingus.png";
        private const string Avatar18 = "predefined_avatar_supertux.png";
        private const string Avatar19 = "predefined_avatar_popcorn.png";
        private const string Avatar20 = "predefined_avatar_rabbit.png";
        private const string Avatar21 = "predefined_avatar_tiger.png";

        private const string Avatar1Title = "Bear";
        private const string Avatar2Title = "Cat 1";
        private const string Avatar3Title = "Cat 2";
        private const string Avatar4Title = "Chipmunk";
        private const string Avatar5Title = "Cuttlefish";
        private const string Avatar6Title = "Dog";
        private const string Avatar7Title = "Fox";
        private const string Avatar8Title = "Giraffe";
        private const string Avatar9Title = "Hedgehog";
        private const string Avatar10Title = "Honeybee";
        private const string Avatar11Title = "Koala";
        private const string Avatar12Title = "Lady Beetle";
        private const string Avatar13Title = "Lion";
        private const string Avatar14Title = "Owl";
        private const string Avatar15Title = "Panda";
        private const string Avatar16Title = "Penguin 1";
        private const string Avatar17Title = "Penguin 2";
        private const string Avatar18Title = "Penguin 3";
        private const string Avatar19Title = "Popcorn";
        private const string Avatar20Title = "Rabbit";
        private const string Avatar21Title = "Tiger";
        


        private ImageSource _avatar;
        public ImageSource Avatar
        {
            get { return _avatar;}
            set { _avatar = value; OnPropertyChanged(); }
        }

        private ImageSource _avatarPreview;
        public ImageSource AvatarPreview
        {
            get { return _avatarPreview; }
            set { _avatarPreview = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PredefinedProfilePicture> Avatars { get; set; }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            //Where do I get the ID of the logged in user
            //Maybe via get /api/Users/Me?
            var userId = 0;



            //Stream imageStream = new FileStream(Avatar1, FileMode.Open);
            //await client.PostProfilePicture(imageStream, userId);

            var profilePicture = await fetcher.GetProfilePicture(userId);

            Avatar = profilePicture == null ? ImageSource.FromFile("ic_professor.png") : ImageSource.FromStream(() => new MemoryStream(profilePicture.Data));
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
