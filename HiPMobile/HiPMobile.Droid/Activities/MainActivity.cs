// /*
//  * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  *
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */

using System;
using Android.App;
using Android.OS;
using Android.Widget;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using Realms;

namespace de.upb.hip.mobile.droid.Activities {
    [Activity(Theme = "@style/AppTheme",
          Label = "HiPMobile.Droid", MainLauncher = false, Icon = "@drawable/icon")]

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
                /*Exhibit exhibit = BusinessEntitiyFactory.CreateBusinessObject<Exhibit> ();
                button.Text = Realm.GetInstance ().All<Exhibit> ().Count ().ToString();
                exhibit.Name = "Dom";
                StringElement category = BusinessEntitiyFactory.CreateBusinessObject<StringElement> ();
                category.Value = "Categorie";
                exhibit.Categories.Add (category);
                Page page = BusinessEntitiyFactory.CreateBusinessObject<Page> ();
                AppertizerPage appertizerPage = BusinessEntitiyFactory.CreateBusinessObject<AppertizerPage> ();
                page.AppertizerPage = appertizerPage;
                exhibit.Pages.Add (page);*/

            };
        }

    }
}