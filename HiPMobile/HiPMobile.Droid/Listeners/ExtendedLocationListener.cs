using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.droid.Helpers;
using de.upb.hip.mobile.pcl.BusinessLayer.Managers;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;
using System.Linq;

namespace de.upb.hip.mobile.droid.Listeners
{
    /// <summary>
    /// This class follows the singleton pattern.
    /// This enables the ExtendedLocationListener to be called in any arbitrary activity without having to move the
    /// binder through several activities or restarting the ExtendedLocationListener everytime (as this is a service).
    /// </summary>
    public class ExtendedLocationListener : Service, ILocationListener
    {
        //for singleton pattern implementation
        static ExtendedLocationListener instance = new ExtendedLocationListener();

        //for the class itself
        private Context context;
        
        public LocationManager locationManager;
        // Flag for GPS status
        public bool CanGetLocation { get; private set; } = false;
        //---------------------------------------------------------------------------------------------------


        private Location location; // Location
        private string LOG_TAG = "ExtendedLocationListener";

        //checking for distance towards exhibits
        private ExhibitSet exhibitSet;
        //only check the ones, that weren't reached or have been out of reach in between. 
        //checkedExhibits contains those exhibits, that will be skipped
        private List<String> checkedExhibits;
        //only check if it is wanted
        private bool checkForExhibitsEnabled = false;


        #region singleton setup
        /// <summary>
        /// Since the ExtendedLocationListener will be created as a singleton from a static context,
        /// it can't get any attributes during creation and neither during getting an instance of it.
        /// Thus, the programmer has to make sure to give the ExtendedLocationListener a context
        /// right after callinge getInstance(), otherwise the class is useless.
        /// </summary>
        private ExtendedLocationListener()
        {
            exhibitSet = ExhibitManager.GetExhibitSet();
            checkedExhibits = new List<String>();
        }

        /// <summary>
        /// Returns the one and only instance of the extended location listener,
        /// iff the caller provides the context and an exhibitSet is set.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ExtendedLocationListener GetInstance()
        {
            return instance;
        }

        public void SetContext(Context context)
        {
            this.context = context;
        }

        public void Initialize(LocationManager locMgr)
        {
            locationManager = locMgr;
            ChangeGpsProvider();
        }

        /// <summary>
        /// unregister resets context and settings. this should be called by every activity when it is destroyed
        /// or doesn't use the ExtendedLocationListener anymore
        /// </summary>
        public void Unregister()
        {
            context = null;
            checkForExhibitsEnabled = false;
            checkedExhibits.Clear();
        }

        #endregion

        public Location GetLocation()
        {
            return location;
        }

        /// <summary>
        /// The longitude of the last know location or 0 otherwise.
        /// </summary>
        private double Longitude
        {
            get
            {
                if (location != null)
                {
                    return location.Longitude;
                }

                return 0;
            }      
        }

        /// <summary>
        /// The latitude of the last known location or 0 otherwise
        /// </summary>
        private double Latitude
        {
            get
            {
                if (location != null)
                {
                    return location.Latitude;
                }

                return 0;
            }       
        }

        public void ShowSettingsAlert()
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(context);

            // Setting Dialog Title
            alertDialog.SetTitle(Resource.String.gps_settings);

            // Setting Dialog Message
            alertDialog.SetMessage(Resource.String.gps_not_enabled_message);

            // On pressing the Settings button.
            alertDialog.SetPositiveButton(Resource.String.settings, (sender, args) =>
            {
                Intent intent = new Intent(Settings.ActionLocationSourceSettings);
                context.StartActivity(intent);
            });

            // On pressing the cancel button
            alertDialog.SetNegativeButton(Resource.String.cancel, (sender, args) =>
            {
                alertDialog.Dispose(); // TODO this seems wrong
            });

        // Showing Alert Message
        alertDialog.Show();
    }

        /// <summary>
        /// Method that must be declared because of the inheritance of the Service class,
        /// but always returns null.
        /// </summary>
        /// <param name="intent">Intent</param>
        /// <returns>null</returns>
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public void OnLocationChanged(Location location)
        {
            this.location = location;

            if(context == null)
            {
                context = Android.App.Application.Context;
            }

            if (checkForExhibitsEnabled)
            {   //even if the activity wants this to check for locations, a new activity can't be startet 
                //without the listener having a context
                CheckVicinityForExhibits();
            }
            
        }

        #region exhibitcheck handling
        public void EnableCheckForExhibits()
        {
            checkForExhibitsEnabled = true;
        }

        public void DisableCheckForExhibits()
        {
            checkForExhibitsEnabled = false;
        }

        /// <summary>
        /// checks the close vicinity for exhibits and asks the user, if he wants to open
        /// the ExhibitDetailsActivity for an exhibit in the vicinity.
        /// "exhibit in the vicinity" means, that the user's gps position is some 10m around
        /// the exhibit location - however an exhibit may be an area and thus needs to be conidered
        /// </summary>
        private void CheckVicinityForExhibits()
        {
            Location curLoc = location;
            foreach (Exhibit e in exhibitSet)
            {
                //following code is to ignore already checked exhibits
                bool disregard = false;
                foreach (String exhibitId in checkedExhibits)
                {
                    if (GetDistance(curLoc, e.Location) >= 0.01 + e.Radius)    //it may be better to let the radius
                                                                    //and the radius only decide, when to
                                                                    //ask to open the exhibit details
                                                                    //however, the standard value is 0, so the
                                                                    //10m will stay for now as a basic value
                    {
                        checkedExhibits.Remove(exhibitId);
                        checkedExhibits.TrimExcess();   //this is to keep the size information accurate
                        disregard = true; //it was just checked, that it outdistances the threshold, no need to check again
                        break;
                    }
                    if (e.Equals(exhibitId))
                    {
                        disregard = true;
                        break;
                    }
                }
                if (disregard)
                {
                    continue;
                }

                if (GetDistance(curLoc, e.Location) < 0.01) //10m
                {   //the exhibit must not be checked with the user, until it was out of reach at least once (distance>=0.01)
                    /*idea: leave it in the list, but start a second list with all exhibits that were reached.
                            check those everytime for surpassing the border, and if they did, put them out of the list
                            check in the if-question, if distance is lower than 10m and if it is not in the extra list 
                            (see checkedExhibits.Remove)*/
                    checkedExhibits.Add(e.Name);

                    //event or popup notification here
                    AlertDialog.Builder alert = new AlertDialog.Builder(context);
                    alert.SetTitle("Exhibit Alert!");
                    alert.SetMessage(Resource.String.exhibit_is_near1 + e.Name + Resource.String.exhibit_is_near2);
                    alert.SetPositiveButton(Resource.String.exhibit_open_yes, (senderAlert, args) => {
                        Toast.MakeText(this.context, "Opened", ToastLength.Short).Show();
                        Intent intent = new Intent(this.context, typeof(ExhibitDetailsActivity));
                        var pageList = e.Pages;
                        if (pageList == null || !pageList.Any())
                        {
                            Toast.MakeText(this.context,
                                            this.context.GetString(Resource.String.currently_no_further_info),
                                            ToastLength.Short).Show();
                        }
                        else
                        {
                            intent.PutExtra(ExhibitDetailsActivity.INTENT_EXTRA_EXHIBIT_ID, e.Id);
                            context.StartActivity(intent);
                        }
                    });

                    alert.SetNegativeButton(Resource.String.exhibit_open_no, (senderAlert, args) => {
                        //Toast.MakeText(this.context, "Canceled!", ToastLength.Short).Show();

                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            }
        }
        #endregion

        #region provider handling
        public void OnProviderDisabled(string provider)
        {
            ChangeGpsProvider();
        }

        public void OnProviderEnabled(string provider)
        {
            ChangeGpsProvider();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            ChangeGpsProvider();
        }

        private void ChangeGpsProvider()
        {
            //get best suited provider
            Criteria locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;

            var locationProvider = locationManager.GetBestProvider(locationCriteria, true);

            if (locationProvider != null)
            {
                locationManager.RequestLocationUpdates(locationProvider, AndroidConstants.MIN_TIME_BW_UPDATES,
                   AndroidConstants.MIN_DISTANCE_CHANGE_FOR_UPDATES, this, Looper.MainLooper);

                location = locationManager.GetLastKnownLocation(locationProvider);  //this is needed especially for the first
                                                                                    //time the location needs to be set
            }
            else
            {
                ShowSettingsAlert();
                Log.Info(LOG_TAG, "No location providers available");
            }
        }
        #endregion

        #region distance calculation
        public double GetDistance(Location a, Location b)
        {
            return DistanceCalculation(a.Latitude,  a.Longitude,  b.Latitude, b.Longitude);
        }

        public double GetDistance(GeoLocation a, Location b)
        {
            return DistanceCalculation(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public double GetDistance(Location a, GeoLocation b)
        {
            return DistanceCalculation(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public double GetDistance(GeoLocation a, GeoLocation b)
        {
            return DistanceCalculation(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }


        //TODO: this is also implemented in the Exhibit.cs (nongenerated) code. 
        //one implementation should suffice in the future (e.g. extra math/util class)
        ///<summary>
        /// distance calculations as presented on http://andrew.hedges.name/experiments/haversine/
        ///</summary>
        //dlon = lon2 - lon1
        //dlat = lat2 - lat1
        //a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
        //c = 2 * atan2(sqrt(a), sqrt(1-a) )
        //d = R* c(where R is the radius of the Earth; 6373km as used in Exhibit class)
        private double DistanceCalculation(double lat1, double long1, double lat2, double long2)
        {
            double dlon = toRadians(long2 - long1);
            double dlat = toRadians(lat2 - lat1);
            double a = Math.Pow(Math.Sin(dlat/2), 2) + Math.Cos(toRadians(lat1) ) * Math.Cos(toRadians(lat2)) * Math.Pow(Math.Sin(dlon/2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6373 * c;
        }

        private double toRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
        #endregion
    }
}