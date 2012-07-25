namespace PushNotification.WebSites.Web.Infrastructure
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.WindowsAzure;
	using Microsoft.WindowsAzure.StorageClient;
	using PushNotification.WebSites.Web.Infrastructure.Helpers;
	using PushNotification.WebSites.Web.Models;

    public class PrivilegesTableServiceContext : TableServiceContext, IUserPrivilegesRepository
    {
        public const string UserPrivilegeTableName = "ExtensiblePNUserPrivileges";
        private const string PublicUserId = "00000000-0000-0000-0000-000000000000";

        public PrivilegesTableServiceContext()
			: this(CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")))
        {
        }

        public PrivilegesTableServiceContext(CloudStorageAccount account)
            : this(account.TableEndpoint.ToString(), account.Credentials)
        {
        }

        public PrivilegesTableServiceContext(string baseAddress, StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
            IgnoreResourceNotFoundException = true;
            IgnoreMissingProperties = true;
        }

        public IQueryable<UserPrivilege> UserPrivileges
        {
            get { return CreateQuery<UserPrivilege>(UserPrivilegeTableName); }
        }

        #region IUserPrivilegesRepository Members

        public IEnumerable<UserPrivilege> GetUsersWithPrivilege(string privilege)
        {
            return this.UserPrivileges
                .Where(p => p.Privilege.Equals(privilege, StringComparison.OrdinalIgnoreCase))
                .AsEnumerable();
        }

        public void AddPrivilegeToUser(string userId, string privilege)
        {
            if (!this.HasUserPrivilege(userId, privilege))
            {
                AddObject(UserPrivilegeTableName, new UserPrivilege { UserId = userId, Privilege = privilege });
                SaveChanges();
            }
        }

        public void AddPublicPrivilege(string privilege)
        {
            this.AddPrivilegeToUser(PublicUserId, privilege);
        }

        public void RemovePrivilegeFromUser(string userId, string privilege)
        {
            UserPrivilege userPrivilege = this.GetUserPrivilege(userId, privilege);
            if (userPrivilege != null)
            {
                DeleteObject(userPrivilege);
                SaveChanges();
            }
        }

        public void DeletePublicPrivilege(string privilege)
        {
            this.RemovePrivilegeFromUser(PublicUserId, privilege);
        }

        public bool HasUserPrivilege(string userId, string privilege)
        {
            return this.GetUserPrivilege(userId, privilege) != null;
        }

        public bool PublicPrivilegeExists(string privilege)
        {
            return this.HasUserPrivilege(PublicUserId, privilege);
        }

        public void DeletePrivilege(string privilege)
        {
            IEnumerable<UserPrivilege> userPrivileges = this.GetUsersWithPrivilege(privilege);
            foreach (UserPrivilege userPrivilege in userPrivileges)
            {
                DeleteObject(userPrivilege);
            }

            SaveChanges();
        }

        #endregion

        private UserPrivilege GetUserPrivilege(string userId, string privilege)
        {
            return this.UserPrivileges
                .Where(
                    p =>
                    p.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) &&
                    p.Privilege.Equals(privilege, StringComparison.OrdinalIgnoreCase))
                .AsEnumerable()
                .FirstOrDefault();
        }
    }
}