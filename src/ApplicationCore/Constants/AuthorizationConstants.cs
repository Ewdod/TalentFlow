using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Constants
{
    public static class AuthorizationConstants
    {
        public const string DEFAULT_ADMIN_USER = "swarley.stinson@bilgeadamboost.com";
        public const string DEFAULT_PASSWORD = "P@ssword1";

        public const string DEFAULT_MANAGER = "barney.stinson@bilgeadam.com";
        public const string DEFAULT_MANAGERPASSWORD = "P@ssword1";
        public static class Roles
        {
            public const string ADMINISTRATOR = "CompanyManager";
            public const string PERSONNEL = "Personnel";
            public const string MANAGER = "Manager";
        }
    }
}
