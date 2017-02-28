// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace de.upb.hip.mobile.pcl.Helpers
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
        /// Show hint for timeslider again
        /// </summary>
        private const string RepeatHintTimeSliderKey = "repeat_hint_time_slider_key";
        private static readonly bool RepeatHintTimeSliderDefault = false;

        public static bool RepeatHintTimeSlider
        {
            get { return AppSettings.GetValueOrDefault<bool>(RepeatHintTimeSliderKey, RepeatHintTimeSliderDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(RepeatHintTimeSliderKey, value); }
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
        #endregion
    }
}