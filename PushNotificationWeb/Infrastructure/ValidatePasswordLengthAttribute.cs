namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web.Security;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' must be at least {1} characters long.";

        private readonly int minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(DefaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, this.ErrorMessageString, name, this.minCharacters);
        }

        public override bool IsValid(object value)
        {
            var valueAsString = value as string;

            return (valueAsString != null) && (valueAsString.Length >= this.minCharacters);
        }
    }
}
