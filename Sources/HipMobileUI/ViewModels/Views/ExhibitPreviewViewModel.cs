﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Resources;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
    {
    class ExhibitPreviewViewModel : NavigationViewModel
        {
            public ExhibitPreviewViewModel (Exhibit exhibit, INearbyExhibitManager exhibitManager)
            {
            Exhibit = exhibit;
            Question = Strings.ExhibitOrRouteNearby_Question_Part1 + " \"" + Exhibit.Name + "\" " + Strings.ExhibitOrRouteNearby_Question_Part2;
            var data = exhibit.Image.Data;
            Image = ImageSource.FromStream (() => new MemoryStream (data));

            Confirm = new Command (Accept);
            Decline = new Command (Deny);

            ExhibitManger = exhibitManager;
            }

        ExhibitVisitedDelegate privateDeletegate;
        INearbyExhibitManager ExhibitManger;

        private Exhibit Exhibit;
        private INearbyExhibitManager ExhibitManager { get; }
        public string Question { set; get; }
        public ImageSource Image { set; get; }
        public ICommand Confirm { get; }
        public ICommand Decline { get; }

        void Accept ()
            {
            MessagingCenter.Send<NavigationViewModel, bool> (this, "ReturnValue", true);
            IoCManager.Resolve<INavigationService> ().PopModalAsync ();
            IoCManager.Resolve<INavigationService> ().PushAsync (new ExhibitDetailsViewModel (Exhibit.Id));
            ExhibitManger.InvokeExhibitVistedEvent (Exhibit);
            }

        void Deny ()
            {
            IoCManager.Resolve<INavigationService> ().PopModalAsync ();
            }

        }
    }
