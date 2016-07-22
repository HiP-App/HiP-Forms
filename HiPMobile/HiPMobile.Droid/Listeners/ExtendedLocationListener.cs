using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Util;
using de.upb.hip.mobile.droid.Activities;
using de.upb.hip.mobile.pcl.BusinessLayer.Models;

namespace de.upb.hip.mobile.droid.Listeners
{
    public class ExtendedLocationListener : Service, ILocationListener
    {
        // The minimum distance to change Updates in meters
        public static readonly long MIN_DISTANCE_CHANGE_FOR_UPDATES = 2; // 2 meters
                                                                      // The minimum time between updates in milliseconds
        public static readonly long MIN_TIME_BW_UPDATES = 2000; // 2 sec
        private Context context;
        // Declaring a Location Manager
        protected LocationManager LocationManager { get; private set; }
        // Flag for GPS status
        public bool CanGetLocation { get; private set; } = false;
        private Location location; // Location
        private string LOG_TAG = "ExtendedLocationListener";
        public static readonly GeoLocation PADERBORN_HBF;

        static ExtendedLocationListener()
        {
            PADERBORN_HBF = new GeoLocation()
            {
                Latitude = 51.71352,
                Longitude = 8.74021
            };
        }

        public ExtendedLocationListener(Context context)
        {  
            this.context = context;
            GetLocation();
        }

        /// <summary>
        /// Returns the current location of the device, if GPS or internet connection is available,
        /// else it returns the last known location or null.
        /// </summary>
        /// <returns>Location</returns>
        public Location GetLocation()
        {
            try
            {
                LocationManager = (LocationManager)context.GetSystemService(LocationService);

                // Getting GPS status
                bool gpsEnabled = LocationManager.IsProviderEnabled(LocationManager.GpsProvider);

                // Getting network status
                bool networkEnabled = LocationManager.IsProviderEnabled(
                        LocationManager.NetworkProvider);

                if (!gpsEnabled && !networkEnabled)
                {
                    // No network provider is enabled
                    CanGetLocation = false;
                }
                else
                {
                    this.CanGetLocation = true;
                    if (networkEnabled)
                    {
                        LocationManager.RequestLocationUpdates(
                                LocationManager.NetworkProvider,
                                MIN_TIME_BW_UPDATES,
                                MIN_DISTANCE_CHANGE_FOR_UPDATES, this);
                        if (LocationManager != null)
                        {
                            location = LocationManager.GetLastKnownLocation(
                                    LocationManager.NetworkProvider);
                        }
                    }

                    // If GPS enabled, get mLatitude/mLongitude using GPS Services
                    if (gpsEnabled)
                    {
                        if (location == null)
                        {
                            LocationManager.RequestLocationUpdates(
                                    LocationManager.GpsProvider,
                                    MIN_TIME_BW_UPDATES,
                                    MIN_DISTANCE_CHANGE_FOR_UPDATES, this);
                            if (LocationManager != null)
                            {
                                location = LocationManager.GetLastKnownLocation(
                                        LocationManager.GpsProvider);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(LOG_TAG, e.Message);
            }

            return location;
        }

        /// <summary>
        /// Stop using GPS listener
        /// Calling this function will stop using GPS in your app.
        /// </summary>
        public void StopUsingGps()
        {
            LocationManager?.RemoveUpdates(this);
        }

       /// <summary>
       /// The longitude of the last know location or 0 otherwise.
       /// </summary>
        public double Longitude
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
        public double Latitude
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

        public void showSettingsAlert()
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
            Activity activity = (Activity)ApplicationContext;
            if (activity.GetType() == typeof(MainActivity)) {
            MainActivity mainActivity = (MainActivity)activity;
            //mainActivity.UpdatePosition(location);
        }
}

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }
    }
}