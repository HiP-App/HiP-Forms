// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using HipMobileUI.Annotations;
using HipMobileUI.Helpers;
using HipMobileUI.Pages;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace HipMobileUI.Viewmodels.MainScreenContainables {
    public class StressTestViewModel : INotifyPropertyChanged {

        private ICommand openManyPages;
        private int numberofPages = 0;
        private ICommand openPageWithManyImages;
        private int numberOfImages;

        public void Init (INavigation navigation)
        {
            int numberOfExhibits = ExhibitManager.GetExhibits ().Count ();
            Random r = new Random();
            this.OpenManyPages = new Command(() => {
                for (int i = 0; i < this.NumberofPages; i++)
                {
                    var exhibit = ExhibitManager.GetExhibits().ElementAt (r.Next(numberOfExhibits));
                    navigation.PushAsync (new ExhibitDetailsPage (exhibit.Id));
                }
            });
            this.OpenPageWithManyImages = new Command (() => {
                var page = new ContentPage ();
                                                           var view = new ScrollView ();
                                                           page.Content = view;
                var layout = new StackLayout ();
                view.Content = layout;
                                                           for (int i = 0; i < this.NumberOfImages; i++)
                                                           {
                                                               var img = new Image {Source = GetRandomImage ()};
                    layout.Children.Add (img);
                                                           }
                                                           navigation.PushAsync (page);
                                                       });
        }

        private ImageSource GetRandomImage ()
        {
            var dataAccess = IoCManager.UnityContainer.Resolve<IDataAccess> ();
            var imageIds = dataAccess.GetItems<de.upb.hip.mobile.pcl.BusinessLayer.Models.Image> ().Select (i => i.Id);
            var numberOfImages = imageIds.Count ();
            Random r = new Random();

            return dataAccess.GetItem<de.upb.hip.mobile.pcl.BusinessLayer.Models.Image> (imageIds.ElementAt(r.Next (numberOfImages))).GetImageSource ();
        }

        public ICommand OpenManyPages {
            get { return openManyPages; }
            set {
                openManyPages = value;
                OnPropertyChanged ();
            }
        }

        public ICommand OpenPageWithManyImages {
            get { return openPageWithManyImages; }
            set {
                openPageWithManyImages = value;
                OnPropertyChanged ();
            }
        }

        public int NumberofPages {
            get { return numberofPages; }
            set {
                numberofPages = value;
                OnPropertyChanged ();
            }
        }

        public int NumberOfImages {
            get { return numberOfImages; }
            set {
                numberOfImages = value;
                OnPropertyChanged ();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
        }

    }
}