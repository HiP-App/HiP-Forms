using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using HipMobileUI.Pages;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace HipMobileUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess>();
            IoCManager.UnityContainer.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            DbManager.UpdateDatabase();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
