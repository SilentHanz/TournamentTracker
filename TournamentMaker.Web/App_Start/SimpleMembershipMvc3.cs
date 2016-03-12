using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Security;
using TournamentReport.Services;
using WebActivatorEx;
using WebMatrix.WebData;

[assembly: PreApplicationStartMethod(typeof (TournamentReport.App_Start.SimpleMembershipMvc3), "Start")]
[assembly: PostApplicationStartMethod(typeof (TournamentReport.App_Start.SimpleMembershipMvc3), "Initialize")]

namespace TournamentReport.App_Start
{
    public static class SimpleMembershipMvc3
    {
        public static readonly string EnableSimpleMembershipKey = "enableSimpleMembership";

        public static bool SimpleMembershipEnabled
        {
            get { return IsSimpleMembershipEnabled(); }
        }

        public static void Initialize()
        {
            var webSecurityService = DependencyResolver.Current.GetService<IWebSecurityService>();
            webSecurityService.InitializeDatabaseConnection(
                connectionStringName: "TournamentReport.TournamentContext",
                userTableName: "Users",
                userIdColumn: "ID",
                userNameColumn: "Name",
                autoCreateTables: true);

            SeedMembershipData(webSecurityService);
        }

        private static void SeedMembershipData(IWebSecurityService webSecurity)
        {
            if (!Roles.RoleExists(Constants.AdministratorsRoleName))
            {
                Roles.CreateRole(Constants.AdministratorsRoleName);
            }
            if (!webSecurity.UserExists("Admin"))
            {
                // TODO: Make sure password reset functionality works.
                webSecurity.CreateUserAndAccount("Admin", GenerateRandomPassword());
                Roles.AddUserToRole("Admin", Constants.AdministratorsRoleName);
            }
        }

        private static string GenerateRandomPassword()
        {
            var data = new byte[32];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }

        public static void Start()
        {
            if (SimpleMembershipEnabled)
            {
                MembershipProvider provider = Membership.Providers["AspNetSqlMembershipProvider"];
                if (provider != null)
                {
                    MembershipProvider currentDefault = provider;
                    SimpleMembershipProvider provider2 =
                        CreateDefaultSimpleMembershipProvider("AspNetSqlMembershipProvider", currentDefault);
                    Membership.Providers.Remove("AspNetSqlMembershipProvider");
                    Membership.Providers.Add(provider2);
                }
                Roles.Enabled = true;
                RoleProvider provider3 = Roles.Providers["AspNetSqlRoleProvider"];
                if (provider3 != null)
                {
                    RoleProvider provider6 = provider3;
                    SimpleRoleProvider provider4 = CreateDefaultSimpleRoleProvider("AspNetSqlRoleProvider", provider6);
                    Roles.Providers.Remove("AspNetSqlRoleProvider");
                    Roles.Providers.Add(provider4);
                }
            }
        }

        #region : Private Methods :

        private static bool IsSimpleMembershipEnabled()
        {
            bool flag;
            string str = ConfigurationManager.AppSettings[EnableSimpleMembershipKey];
            if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out flag))
            {
                return flag;
            }
            return true;
        }

        private static SimpleMembershipProvider CreateDefaultSimpleMembershipProvider(string name,
                                                                                      MembershipProvider currentDefault)
        {
            MembershipProvider previousProvider = currentDefault;
            SimpleMembershipProvider provider = new SimpleMembershipProvider(previousProvider);
            NameValueCollection config = new NameValueCollection();
            provider.Initialize(name, config);
            return provider;
        }

        private static SimpleRoleProvider CreateDefaultSimpleRoleProvider(string name, RoleProvider currentDefault)
        {
            RoleProvider previousProvider = currentDefault;
            SimpleRoleProvider provider = new SimpleRoleProvider(previousProvider);
            NameValueCollection config = new NameValueCollection();
            provider.Initialize(name, config);
            return provider;
        }

        #endregion
    }
}