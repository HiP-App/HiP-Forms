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

    }
}