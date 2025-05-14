using System.Text;
using System.Text.Json;
using ColorValley.Helper;
using iJus.Core.Settings;

namespace ColorValley.Settings
{
    /// <summary>
    /// Holds the current user settings
    /// </summary>
    public class UserSettings : IUserSettings
    {
        internal const string AesKey = "/Wb/ZrmA/4MyZTkiAPqDdlj+eckptlIXSFL7G3LcPW0=";
        internal const string AesIV = "WPKR4co1m/7NFLIDi2QQqw==";

        /// <summary>
        /// The name of the online content file.
        /// </summary>
        public const string UserSettingsFile = "color_valley_usersettings.clval";

        /// <summary>
        /// Gets the local content file path.
        /// </summary>
        public static string GetUserSettingsFilePath()
        {
            return Path.Combine(ContentHelper.ContentFolder, UserSettingsFile);
        }

        /// <summary>
        /// Loads the user settings.
        /// </summary>
        /// <param name="filePath">The file path from which the settings should be loaded.</param>
        public static TUserSettings? LoadDecrypted<TUserSettings>(string filePath) where TUserSettings : IUserSettings, new()
        {

            if (!File.Exists(filePath))
            {
                var newUserSettings = new TUserSettings();

                newUserSettings.SaveEncrypted(filePath);
                return newUserSettings;
            }

            var encryptedText = File.ReadAllText(filePath);
            var decryptedText = CryptoHelper.DecryptBase64TextWithAes(encryptedText, AesKey, AesIV);

            return JsonSerializer.Deserialize<TUserSettings>(decryptedText);
        }

        /// <summary>
        /// Loads the user settings from the default user settings file path.
        /// </summary>
        public static TUserSettings? LoadDecrypted<TUserSettings>() where TUserSettings : IUserSettings, new()
        {
            return LoadDecrypted<TUserSettings>(GetUserSettingsFilePath());
        }
    }
}