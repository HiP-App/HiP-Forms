using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.ModelClasses {
    public class UserRating : RealmObject {
        //Attributes
        [PrimaryKey]
        public int Id { get; set; }

        public double Average { get; set; }

        public int Count { get; set; }
    }
}