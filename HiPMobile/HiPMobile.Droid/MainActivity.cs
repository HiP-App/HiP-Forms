using System;
using Android.App;
using Android.OS;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
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

            //Delete current database to avoid migration issues, remove this when wanting persistent database usage
            Realm.DeleteRealm (new RealmConfiguration ());

            button.Click += (sender, args) => {
                // Testing BusinessEntities
                Exhibit exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit> ();
                button.Text = Realm.GetInstance ().All<Exhibit> ().Count ().ToString();
                exhibit.Name = "Dom";
                StringElement category = BusinessEntitiyFactory.CreateBusinessObject<StringElement> ();
                category.Value = "Categorie";
                exhibit.Categories.Add (category);
                Page page = BusinessEntitiyFactory.CreateBusinessObject<Page> ();
                AppertizerPage appertizerPage = BusinessEntitiyFactory.CreateBusinessObject<AppertizerPage> ();
                page.AppertizerPage = appertizerPage;
                exhibit.Pages.Add (page);

            };
        }

    }
}