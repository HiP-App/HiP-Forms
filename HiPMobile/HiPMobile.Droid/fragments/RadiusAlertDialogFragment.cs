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

using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.fragments {
    public class RadiusAlertDialogFragment : DialogFragment {

        public const string Data = "DATA";
        private Exhibit e;

        public static RadiusAlertDialogFragment NewInstance ()
        {
            var fragment = new RadiusAlertDialogFragment ();
            var args = new Bundle ();
            return fragment;
        }

        public override Dialog OnCreateDialog (Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
                e = ExhibitManager.GetExhibit (savedInstanceState.GetString (Data));

            var alert = new AlertDialog.Builder (Activity);
            alert.SetTitle ("Exhibit Alert!");
            alert.SetMessage (Activity.Resources.GetString (Resource.String.exhibit_is_near1) + e.Name + Activity.Resources.GetString (Resource.String.exhibit_is_near2));
            alert.SetPositiveButton (Resource.String.exhibit_open_yes, (senderAlert, args) => {
                                         Toast.MakeText (Activity, "Opened", ToastLength.Short).Show ();
                                         var intent = new Intent (Activity, typeof (ExhibitDetailsActivity));
                                         var pageList = e.Pages;
                                         if ((pageList == null) || !pageList.Any ())
                                         {
                                             Toast.MakeText (Activity,
                                                             Activity.GetString (Resource.String.currently_no_further_info),
                                                             ToastLength.Short).Show ();
                                         }
                                         else
                                         {
                                             intent.PutExtra (ExhibitDetailsActivity.INTENT_EXTRA_EXHIBIT_ID, e.Id);
                                             Activity.StartActivity (intent);
                                         }
                                     });

            alert.SetNegativeButton (Resource.String.exhibit_open_no, (senderAlert, args) => { Toast.MakeText (Activity, "Canceled!", ToastLength.Short).Show (); });

            return alert.Create ();
        }


        public void SetExhibit (Exhibit e)
        {
            this.e = e;
        }


        public override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);

            outState.PutString (Data, e.Id);
        }

    }
}