// Helpers/Settings.cs

using System;
using System.ComponentModel;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System.Runtime.CompilerServices;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public sealed class Events : INotifyPropertyChanged
        {
            public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        /// <summary>
        /// Some properties in this class notify this instance
        /// about changes to them.
        /// </summary>
        public static Events ChangeEvents = new Events();

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion


        public static string GeneralSettings
        {
            get => AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            set => AppSettings.AddOrUpdateValue(SettingsKey, value);
        }

        /// <summary>
        /// Sedtting for the database version.
        /// </summary>
        #region MyRegion
        private const string DatabaseVersionKey = "database_version_key";
        private static readonly int DatabaseVersionDefault = 0;

        public static int DatabaseVersion
        {
            get => AppSettings.GetValueOrDefault(DatabaseVersionKey, DatabaseVersionDefault);
            set => AppSettings.AddOrUpdateValue(DatabaseVersionKey, value);
        }
        #endregion

        /// <summary>
        /// Settings from the Settings Screen.
        /// </summary>
        #region SettingsScreen

        /// <summary>
        /// After end of audio playback, switch automatically to next page
        /// </summary>
        private const string AutoSwitchPageKey = "auto_switch_page_key";
        private static readonly bool AutoSwitchPageDefault = false;

        public static bool AutoSwitchPage
        {
            get => AppSettings.GetValueOrDefault(AutoSwitchPageKey, AutoSwitchPageDefault);
            set => AppSettings.AddOrUpdateValue(AutoSwitchPageKey, value);
        }

        /// <summary>
        /// Automatically start audio playback for current page
        /// </summary>
        private const string AutoStartAudioKey = "auto_start_audio_key";
        private static readonly bool AutoStartAudioDefault = false;

        public static bool AutoStartAudio
        {
            get => AppSettings.GetValueOrDefault(AutoStartAudioKey, AutoStartAudioDefault);
            set => AppSettings.AddOrUpdateValue(AutoStartAudioKey, value);
        }

        /// <summary>
        /// Show hint for audio playback again
        /// </summary>
        private const string RepeatHintAudioKey = "repeat_hint_audio_key";
        private static readonly bool RepeatHintAudioDefault = true;

        public static bool RepeatHintAudio
        {
            get => AppSettings.GetValueOrDefault(RepeatHintAudioKey, RepeatHintAudioDefault);
            set => AppSettings.AddOrUpdateValue(RepeatHintAudioKey, value);
        }

        /// <summary>
        /// Show hint for automatically switching to next page again
        /// </summary>
        private const string RepeatHintAutoPageSwitchKey = "repeat_hint_auto_page_switch_key";
        private static readonly bool RepeatHintAutoPageSwitchDefault = true;

        public static bool RepeatHintAutoPageSwitch
        {
            get => AppSettings.GetValueOrDefault(RepeatHintAutoPageSwitchKey, RepeatHintAutoPageSwitchDefault);
            set => AppSettings.AddOrUpdateValue(RepeatHintAutoPageSwitchKey, value);
        }

        /// <summary>
        /// Show app introduction by restarting the app again
        /// </summary>
        private const string RepeatIntroKey = "repeat_intro_key";
        private static readonly bool RepeatIntroDefault = true;

        public static bool RepeatIntro
        {
            get => AppSettings.GetValueOrDefault(RepeatIntroKey, RepeatIntroDefault);
            set => AppSettings.AddOrUpdateValue(RepeatIntroKey, value);
        }

        /// <summary>
        /// Always download new data if available
        /// </summary>
        private const string DownloadDataKey = "download_data_key";
        private static readonly bool DownloadDataDefault = true;

        public static bool AlwaysDownloadData
        {
            get => AppSettings.GetValueOrDefault(DownloadDataKey, DownloadDataDefault);
            set => AppSettings.AddOrUpdateValue(DownloadDataKey, value);
        }

        /// <summary>
        /// Font size used for the audio transcripts
        /// </summary>
        private const string AudioTranscriptFontSizeKey = "audio_transcript_font_size_key";
        private static readonly double AudioTranscriptFontSizeDefault = 19;

        public static double AudioTranscriptFontSize
        {
            get => AppSettings.GetValueOrDefault(AudioTranscriptFontSizeKey, AudioTranscriptFontSizeDefault);
            set => AppSettings.AddOrUpdateValue(AudioTranscriptFontSizeKey, value);
        }

        /// <summary>
        /// Download data only over wifi
        /// </summary>
        private const string WifiOnlyKey = "wifi_only_key";
        private static readonly bool WifiOnlyDefault = true;

        public static bool WifiOnly
        {
            get => AppSettings.GetValueOrDefault(WifiOnlyKey, WifiOnlyDefault);
            set => AppSettings.AddOrUpdateValue(WifiOnlyKey, value);
        }


        private const string AdventurerModeKey = "adventurer_mode_key";
        private static readonly bool AdventurerModeDefault = true;

        /// <summary>
        /// Indicates wether the app is in adventurer or professor mode.
        /// True: App is in Adventurer mode.
        /// False: App is in Professor mode.
        /// </summary>
        public static bool AdventurerMode
        {
            get => AppSettings.GetValueOrDefault(AdventurerModeKey, AdventurerModeDefault);
            set => AppSettings.AddOrUpdateValue(AdventurerModeKey, value);
        }

        private const string InitialThemeSelectedKey = "initial_theme_selected_key";
        private static readonly bool InitialThemeSelectedDefault = false;

        public static bool InitialThemeSelected
        {
            get => AppSettings.GetValueOrDefault(InitialThemeSelectedKey, InitialThemeSelectedDefault);
            set => AppSettings.AddOrUpdateValue(InitialThemeSelectedKey, value);
        }
        #endregion

        /// <summary>
        /// User data
        /// </summary>
        #region UserData

        /// <summary>
        /// Indicator flag if a user is logged in
        /// </summary>
        private const string IsLoggedInKey = "is_logged_in_key";
        private static readonly bool IsLoggedInDefault = false;

        public static bool IsLoggedIn
        {
            get => AppSettings.GetValueOrDefault(IsLoggedInKey, IsLoggedInDefault);
            set
            {
                AppSettings.AddOrUpdateValue(IsLoggedInKey, value);
                ChangeEvents.NotifyPropertyChanged(nameof(IsLoggedIn));
            }
        }

        /// <summary>
        /// The username of the current user
        /// </summary>
        private const string UserNameKey = "username_key";
        private static readonly string UsernameDefault = "Max Power";

        public static string Username
        {
            get => AppSettings.GetValueOrDefault(UserNameKey, UsernameDefault);
            set => AppSettings.AddOrUpdateValue(UserNameKey, value);
        }

        /// <summary>
        /// The e-mail of the current user
        /// </summary>
        private const string EMailKey = "email_key";
        private static readonly string EMailDefault = "max@power.com";

        public static string EMail
        {
            get => AppSettings.GetValueOrDefault(EMailKey, EMailDefault);
            set => AppSettings.AddOrUpdateValue(EMailKey, value);
        }

        /// <summary>
        /// The password of the current user
        /// </summary>
        private const string PasswordKey = "password_key";
        private static readonly string PasswordDefault = "";

        public static string Password
        {
            get => AppSettings.GetValueOrDefault(PasswordKey, PasswordDefault);
            set => AppSettings.AddOrUpdateValue(PasswordKey, value);
        }

        /// <summary>
        /// The access token returned from the auth api after a successful login
        /// </summary>
        private const string AccessTokenKey = "access_token_key";
        private static readonly string AccessTokenDefault = "";

        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault(AccessTokenKey, AccessTokenDefault);
            set => AppSettings.AddOrUpdateValue(AccessTokenKey, value);
        }

        private const string GenericTokenKey = "generic_token_key";
        private const string GenericTokenDefault = "";

        public static string GenericToken
        {
            get => AppSettings.GetValueOrDefault(GenericTokenKey, GenericTokenDefault);
            set => AppSettings.AddOrUpdateValue(GenericTokenKey, value);
        }



        /// <summary>
        /// The score of the current user
        /// </summary>
        private const string ScoreKey = "score_key";
        private static readonly int ScoreDefault = 420;

        public static int Score
        {
            get => AppSettings.GetValueOrDefault(ScoreKey, ScoreDefault);
            set => AppSettings.AddOrUpdateValue(ScoreKey, value);
        }

        /// <summary>
        /// The number of gained achievements of the current user
        /// </summary>
        private const string AchievementsKey = "achievements_key";
        private static readonly int AchievementsDefault = 12;

        public static int Achievements
        {
            get => AppSettings.GetValueOrDefault(AchievementsKey, AchievementsDefault);
            set => AppSettings.AddOrUpdateValue(AchievementsKey, value);
        }

        /// <summary>
        /// The part of everything you can do with the app as a percentage
        /// </summary>
        private const string CompletenessKey = "completeness_key";
        private static readonly int CompletenessDefault = 10;

        public static int Completeness
        {
            get => AppSettings.GetValueOrDefault(CompletenessKey, CompletenessDefault);
            set => AppSettings.AddOrUpdateValue(CompletenessKey, value);
        }

        #endregion

        private const string ShouldDeleteDbOnLaunchKey = "should_delete_db_on_launch_key";
        private static readonly string ShouldDeleteDbOnLaunchDefault = false.ToString();

        /// <summary>
        /// If set to true, the IDataAccess database is deleted on next app launch.
        /// </summary>
        public static bool ShouldDeleteDbOnLaunch
        {
            get => bool.Parse(AppSettings.GetValueOrDefault(ShouldDeleteDbOnLaunchKey, ShouldDeleteDbOnLaunchDefault));
            set => AppSettings.AddOrUpdateValue(ShouldDeleteDbOnLaunchKey, value.ToString());
        }

        /// <summary>
        /// If set to true, the user cannot chose between the two modes.
        /// </summary>
        public static bool DisableAdventurerMode = true;
    }
}