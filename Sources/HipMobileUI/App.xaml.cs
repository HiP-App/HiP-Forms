﻿using System.Linq;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Routing;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.Common.Contracts;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using HipMobileUI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HipMobileUI
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            // Handle when your app starts

            // Setup IoC and database
            IoCManager.RegisterType<IDataAccess, RealmDataAccess>();
            IoCManager.RegisterType<IDataLoader, EmbeddedResourceDataLoader>();
            DbManager.UpdateDatabase();
            var calculator = RouteCalculator.Instance;
            MainPage = new MainPage ();
        }

        protected override void OnStart()
        {
            
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
