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
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
        
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
            get
            {
                return AppSettings.GetValueOrDefault<string>(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(SettingsKey, value);
            }
        }

        /// <summary>
        /// Sedtting for the database version.
        /// </summary>
        #region MyRegion
        private const string DatabaseVersionKey = "database_version_key";
        private static readonly int DatabaseVersionDefault = 0;

        public static int DatabaseVersion
        {
            get { return AppSettings.GetValueOrDefault<int>(DatabaseVersionKey, DatabaseVersionDefault); }
            set { AppSettings.AddOrUpdateValue<int>(DatabaseVersionKey, value); }
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
            get { return AppSettings.GetValueOrDefault<bool>(AutoSwitchPageKey, AutoSwitchPageDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(AutoSwitchPageKey, value); }
        }

        /// <summary>
        /// Automatically start audio playback for current page
        /// </summary>
        private const string AutoStartAudioKey = "auto_start_audio_key";
        private static readonly bool AutoStartAudioDefault = false;

        public static bool AutoStartAudio
        {
            get { return AppSettings.GetValueOrDefault<bool>(AutoStartAudioKey, AutoStartAudioDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(AutoStartAudioKey, value); }
        }

        /// <summary>
        /// Show hint for audio playback again
        /// </summary>
        private const string RepeatHintAudioKey = "repeat_hint_audio_key";
        private static readonly bool RepeatHintAudioDefault = true;

        public static bool RepeatHintAudio
        {
            get { return AppSettings.GetValueOrDefault<bool>(RepeatHintAudioKey, RepeatHintAudioDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(RepeatHintAudioKey, value); }
        }

        /// <summary>
        /// Show hint for automatically switching to next page again
        /// </summary>
        private const string RepeatHintAutoPageSwitchKey = "repeat_hint_auto_page_switch_key";
        private static readonly bool RepeatHintAutoPageSwitchDefault = true;

        public static bool RepeatHintAutoPageSwitch
        {
            get { return AppSettings.GetValueOrDefault<bool>(RepeatHintAutoPageSwitchKey, RepeatHintAutoPageSwitchDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(RepeatHintAutoPageSwitchKey, value); }
        }

        /// <summary>
        /// Show app introduction by restarting the app again
        /// </summary>
        private const string RepeatIntroKey = "repeat_intro_key";
        private static readonly bool RepeatIntroDefault = true;

        public static bool RepeatIntro
        {
            get { return AppSettings.GetValueOrDefault<bool>(RepeatIntroKey, RepeatIntroDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(RepeatIntroKey, value); }
        }

        /// <summary>
        /// Always download new data if available
        /// </summary>
        private const string DownloadDataKey = "download_data_key";
        private static readonly bool DownloadDataDefault = true;

        public static bool AlwaysDownloadData
        {
            get { return AppSettings.GetValueOrDefault<bool>(DownloadDataKey, DownloadDataDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(DownloadDataKey, value); }
        }

        /// <summary>
        /// Font size used for the audio transcripts
        /// </summary>
        private const string AudioTranscriptFontSizeKey = "audio_transcript_font_size_key";
        private static readonly double AudioTranscriptFontSizeDefault = 19;

        public static double AudioTranscriptFontSize
        {
            get { return AppSettings.GetValueOrDefault<double>(AudioTranscriptFontSizeKey, AudioTranscriptFontSizeDefault); }
            set { AppSettings.AddOrUpdateValue<double>(AudioTranscriptFontSizeKey, value); }
        }

        /// <summary>
        /// Download data only over wifi
        /// </summary>
        private const string WifiOnlyKey = "wifi_only_key";
        private static readonly bool WifiOnlyDefault = true;

        public static bool WifiOnly
        {
            get { return AppSettings.GetValueOrDefault<bool>(WifiOnlyKey, WifiOnlyDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(WifiOnlyKey, value); }
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
            get { return AppSettings.GetValueOrDefault<bool>(AdventurerModeKey, AdventurerModeDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(AdventurerModeKey, value); }
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
            get { return AppSettings.GetValueOrDefault(IsLoggedInKey, IsLoggedInDefault); }
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
            get { return AppSettings.GetValueOrDefault(UserNameKey, UsernameDefault); }
            set { AppSettings.AddOrUpdateValue(UserNameKey, value); }
        }

        /// <summary>
        /// The e-mail of the current user
        /// </summary>
        private const string EMailKey = "email_key";
        private static readonly string EMailDefault = "max@power.com";

        public static string EMail
        {
            get { return AppSettings.GetValueOrDefault(EMailKey, EMailDefault); }
            set { AppSettings.AddOrUpdateValue(EMailKey, value); }
        }

        /// <summary>
        /// The password of the current user
        /// </summary>
        private const string PasswordKey = "password_key";
        private static readonly string PasswordDefault = "";

        public static string Password
        {
            get { return AppSettings.GetValueOrDefault(PasswordKey, PasswordDefault); }
            set { AppSettings.AddOrUpdateValue(PasswordKey, value); }
        }

        /// <summary>
        /// The access token returned from the auth api after a successful login
        /// </summary>
        private const string AccessTokenKey = "access_token_key";
        private static readonly string AccessTokenDefault = "";

        public static string AccessToken
        {
            get { return AppSettings.GetValueOrDefault(AccessTokenKey, AccessTokenDefault); }
            set { AppSettings.AddOrUpdateValue(AccessTokenKey, value); }
        }



        /// <summary>
        /// The score of the current user
        /// </summary>
        private const string ScoreKey = "score_key";
        private static readonly int ScoreDefault = 420;

        public static int Score
        {
            get { return AppSettings.GetValueOrDefault(ScoreKey, ScoreDefault); }
            set { AppSettings.AddOrUpdateValue(ScoreKey, value); }
        }

        /// <summary>
        /// The number of gained achievements of the current user
        /// </summary>
        private const string AchievementsKey = "achievements_key";
        private static readonly int AchievementsDefault = 12;

        public static int Achievements
        {
            get { return AppSettings.GetValueOrDefault(AchievementsKey, AchievementsDefault); }
            set { AppSettings.AddOrUpdateValue(AchievementsKey, value); }
        }

        /// <summary>
        /// The part of everything you can do with the app as a percentage
        /// </summary>
        private const string CompletenessKey = "completeness_key";
        private static readonly int CompletenessDefault = 10;

        public static int Completeness
        {
            get { return AppSettings.GetValueOrDefault(CompletenessKey, CompletenessDefault); }
            set { AppSettings.AddOrUpdateValue(CompletenessKey, value); }
        }

        #endregion
    }
}