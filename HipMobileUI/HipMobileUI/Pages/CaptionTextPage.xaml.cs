using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using Xamarin.Forms;

namespace HipMobileUI.Pages
{
    public partial class CaptionTextPage : ContentPage
    {
        public CaptionTextPage()
        {
            InitializeComponent();
            LoadCaptions ();
        }

        public void LoadCaptions()
        {
            var id = ExhibitManager.GetExhibitSet().Last();
            string text = id.Pages[1].Audio.Caption;
            Caption.Text = text;
        }
    }
}
