using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views {
    public class UserRatingListItemViewModel : NavigationViewModel {

        private string userName;
        public string UserName {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private string date;
        public string Date {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private string rating;
        public string Rating {
            get { return rating; }
            set { SetProperty(ref rating, value); }
        }

        private string text;
        public string Text {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        public UserRatingListItemViewModel(Exhibit exhibit) {
            UserName = "Name";
            Date = "20.16.2016";
            rating = "★★★★☆";
            Text = "Some exhibit description...";
        }

    }
}