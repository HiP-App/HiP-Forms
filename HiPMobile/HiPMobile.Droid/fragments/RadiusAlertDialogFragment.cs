using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.fragments
{
    public class RadiusAlertDialogFragment : Android.App.DialogFragment
    {
        Exhibit e;
        public  const string data = "DATA";
        public static RadiusAlertDialogFragment NewInstance()
        {
            RadiusAlertDialogFragment fragment = new RadiusAlertDialogFragment();
            Bundle args = new Bundle();
            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            if(savedInstanceState != null)
                e = ExhibitManager.GetExhibit(savedInstanceState.GetString(data));

            AlertDialog.Builder alert = new AlertDialog.Builder(Activity);
            alert.SetTitle("Exhibit Alert!");
            alert.SetMessage(Activity.Resources.GetString(Resource.String.exhibit_is_near1) +e.Name+ Activity.Resources.GetString(Resource.String.exhibit_is_near2));
            alert.SetPositiveButton(Resource.String.exhibit_open_yes, (senderAlert, args) => {
                Toast.MakeText(Activity, "Opened", ToastLength.Short).Show();
                Intent intent = new Intent(Activity, typeof(ExhibitDetailsActivity));
                var pageList = e.Pages;
                if (pageList == null || !pageList.Any())
                {
                    Toast.MakeText(Activity,
                                    this.Activity.GetString(Resource.String.currently_no_further_info),
                                    ToastLength.Short).Show();
                }
                else
                {
                    intent.PutExtra(ExhibitDetailsActivity.INTENT_EXTRA_EXHIBIT_ID, e.Id);
                    Activity.StartActivity(intent);
                }
            });

            alert.SetNegativeButton(Resource.String.exhibit_open_no, (senderAlert, args) => {
                Toast.MakeText(Activity, "Canceled!", ToastLength.Short).Show();

            });

            return alert.Create();
        }


        public void SetExhibit(Exhibit e)
        {
            this.e = e;
        }


public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString(data, e.Id);
        }
    }
}