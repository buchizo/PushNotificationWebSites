namespace PushNotification.WebSites.Web.Infrastructure.Helpers
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public static class ConfigReader
    {
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "This method gets a configuration setting value.")]
        public static string GetConfigValue(string key, bool validate = true)
        {
            string value = RoleEnvironment.IsAvailable
                               ? RoleEnvironment.GetConfigurationSettingValue(key)
                               : ConfigurationManager.AppSettings[key];

            if (validate)
            {
                Validate(key, value);
            }

            return value;
        }

        public static string GetWebConfigValue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            Validate(key, value);

            return value;
        }

        private static void Validate(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The '{0}' setting is not available.", key), "key");
            }
        }
    }
}