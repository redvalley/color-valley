namespace ColorValley.Settings
{
    /// <summary>
    /// Helper class that holds content related helper methods.
    /// </summary>
    public static class ContentHelper
    {
        /// <summary>
        /// Gets the system specific folder in which the app content should be stored/updated.
        /// </summary>
        /// <value>
        /// The content folder.
        /// </value>
        public static string ContentFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);



    }
}
