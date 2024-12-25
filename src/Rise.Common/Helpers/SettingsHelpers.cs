using System;
using Windows.Storage;

namespace Rise.Common.Helpers
{
    /// <summary>
    /// Contains helper methods to get and set app settings.
    /// </summary>
    public static class SettingsHelpers
    {
        private static readonly ApplicationDataContainer LocalSettings
            = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// Gets the value of an app setting stored locally.
        /// </summary>
        /// <param name="defaultValue">Default value for the setting - see remarks.</param>
        /// <param name="container">The name of the settings container to get the setting from.</param>
        /// <param name="setting">The name of the setting.</param>
        /// <returns>The setting value if it already exists, the default value otherwise.</returns>
        /// <remarks>If the setting doesn't exist yet, the default value is saved before
        /// returning.</remarks>
        /// <exception cref="InvalidCastException">Thrown if the setting cannot be casted to
        /// <typeparamref name="T"/>.</exception>
        public static T GetLocal<T>(T defaultValue, string container, string setting)
        {
            // Get the container values, always create it if it doesn't exist
            var values = LocalSettings.CreateContainer(container, ApplicationDataCreateDisposition.Always).Values;

            values[setting] ??= defaultValue;
            var value = (T)values[setting];

            return value;
        }

        /// <summary>
        /// Sets the value of an app setting stored locally.
        /// </summary>
        /// <param name="newValue">New value for the setting.</param>
        /// <param name="container">The name of the settings container this setting will
        /// be stored in.</param>
        /// <param name="setting">The name of the setting.</param>
        /// <exception cref="ArgumentException">Thrown if the currently stored setting
        /// is not an instance of <typeparamref name="T"/>.</exception>
        public static void SetLocal<T>(T newValue, string container, string setting)
        {
            // Get the container, always create it if it doesn't exist
            var values = LocalSettings.CreateContainer(container, ApplicationDataCreateDisposition.Always).Values;

            // Check whether type matches
            object value = values[setting];
            if (value is not T && value != null)
            {
                string message = $"Type mismatch for \"{setting}\" in \"{container}\" container. Current type is {value.GetType()}";
                throw new ArgumentException(message, nameof(setting));
            }

            // Set the setting to the desired value
            values[setting] = newValue;
        }
    }
}
