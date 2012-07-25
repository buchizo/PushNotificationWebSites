namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using PushNotification.WebSites.Web.Models;

    [CLSCompliant(false)]
    public interface IUserPrivilegesRepository
    {
        IEnumerable<UserPrivilege> GetUsersWithPrivilege(string privilege);

        void AddPrivilegeToUser(string userId, string privilege);

        void AddPublicPrivilege(string privilege);

        void RemovePrivilegeFromUser(string userId, string privilege);

        void DeletePublicPrivilege(string privilege);

        void DeletePrivilege(string privilege);

        bool HasUserPrivilege(string userId, string privilege);

        bool PublicPrivilegeExists(string privilege);
    }
}