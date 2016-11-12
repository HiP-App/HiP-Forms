// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace de.upb.hip.mobile.droid.Dialogs {
    public class HelpDialogFragment : DialogFragment, IDialogInterfaceOnDismissListener {

        public enum HelpWindows {

            AutomaticAudioHelp,
            AutomaticSwitchHelp

        }


        public static HelpDialogFragment Fragment;
        private static AlertDialog.Builder alert;
        private string message;

        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor sharedPreferencesEditor;

        public static Action OnCloseDialogAction { get; set; }

        public static HelpWindows Type { get; set; }

        /*
        public override void OnDismiss (IDialogInterface dialog)
        {

            //OnCloseDialogAction ();
            Activity.FragmentManager.BeginTransaction().Remove(this).Commit();
        }
        */

        public static HelpDialogFragment NewHelpDialogFragment (HelpWindows helpWindowType, Action onCloseDialogAction)
        {
            Fragment = new HelpDialogFragment ();
            Type = helpWindowType;
            OnCloseDialogAction = onCloseDialogAction;
            return Fragment;
        }


        public override Dialog OnCreateDialog (Bundle savedInstanceState)
        {
            alert = new AlertDialog.Builder (Activity, Resource.Style.HelpDialogTheme);

            alert.SetTitle (Resources.GetString(Resource.String.hint_message));

            alert.SetIcon(Resource.Drawable.hiphop_transparent);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Activity);
            sharedPreferencesEditor = sharedPreferences.Edit ();

            switch (Type)
            {
                case HelpWindows.AutomaticAudioHelp:
                    message = Resources.GetString (Resource.String.auto_audio_message);

                    // once the user clicks on any dialog option, onboarding is considered finised, and thus the onboaring values are set to false

                    alert.SetPositiveButton (Resources.GetString (Resource.String.keep_feature_on), (senderAlert, args) => {
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_start_audio_key_onboarding), false);
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_start_audio_key), true);
                        sharedPreferencesEditor.Commit ();

                        Toast.MakeText (Activity, Resources.GetString (Resource.String.choice_keep_audio), ToastLength.Short).Show ();
                    });

                    alert.SetNegativeButton (Resources.GetString (Resource.String.disregard_feature), (senderAlert, args) => {
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_start_audio_key_onboarding), false);
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_start_audio_key), false);
                        sharedPreferencesEditor.Commit ();

                        Toast.MakeText (Activity, Resources.GetString (Resource.String.choice_disregard_audio), ToastLength.Short).Show ();
                    });
                    break;

                case HelpWindows.AutomaticSwitchHelp:
                    message = Resources.GetString (Resource.String.auto_switch_message);

                    alert.SetPositiveButton (Resources.GetString (Resource.String.keep_feature_on), (senderAlert, args) => {
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_switch_page_key_onboarding), false);
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_page_switch_key), true);
                        sharedPreferencesEditor.Commit ();
                        Toast.MakeText (Activity, Resources.GetString (Resource.String.choice_keep_switch_pages), ToastLength.Short).Show ();
                    });

                    alert.SetNegativeButton (Resources.GetString (Resource.String.disregard_feature), (senderAlert, args) => {
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_switch_page_key_onboarding), false);
                        sharedPreferencesEditor.PutBoolean (Resources.GetString (Resource.String.pref_auto_page_switch_key), false);
                        sharedPreferencesEditor.Commit ();
                        Toast.MakeText (Activity, Resources.GetString (Resource.String.choice_disregard_switch_pages), ToastLength.Short).Show ();
                    });
                    break;
                default:
                    break;
            }

            alert.SetMessage (message);
            return alert.Create ();
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetCanceledOnTouchOutside (false);
            return base.OnCreateView (inflater, container, savedInstanceState);
        }

    }
}