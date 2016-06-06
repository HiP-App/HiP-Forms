using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using de.upb.hip.mobile.pcl.Common;
using de.upb.hip.mobile.pcl.DataAccessLayer;
using de.upb.hip.mobile.pcl.DataLayer;
using Microsoft.Practices.Unity;
using Realms;

namespace de.upb.hip.mobile.droid {
    [Activity (Label = "HiPMobile.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            
            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button> (Resource.Id.myButton);

            // Setup IoC
            IoCManager.UnityContainer.RegisterType<IDataAccess, RealmDataAccess> ();

            //Delete current database to avoid migration issues, remove this when wanting persistent database usage
            Realm.DeleteRealm (new RealmConfiguration ());

            Exhibit exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit>();
            button.Text = Realm.GetInstance().All<Exhibit>().Count().ToString();
            
            exhibit.Name = "Dom";
            StringElement category = BusinessEntitiyFactory.CreateBusinessObject<StringElement>();
            category.Value = "Categorie";
            exhibit.Categories.Add(category);
            Page page = BusinessEntitiyFactory.CreateBusinessObject<Page>();
            exhibit.Pages.Add(page);
            
            button.Click += (sender, args) => {
                
                // Testing BusinessEntities
                var ex = ExhibitManager.GetExhibits ().First ();
                BusinessEntitiyFactory.DeleteBusinessEntity (ex);
                button.Text = Realm.GetInstance().All<Exhibit>().Count().ToString();
            };
        }

    }
}