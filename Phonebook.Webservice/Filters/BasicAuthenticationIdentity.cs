using System.Security.Principal;

namespace Phonebook.Webservice.Filters
{
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string username, string password) : base(username, "Basic")
        {
            this.password = password;
        }

        public BasicAuthenticationIdentity(string username, string password, string token) : base(username, "Basic")
        {
            this.password = password;
            this.token = token;
        }

        /// <summary>
        /// Basic Auth Password for custom authentication
        /// </summary>
        public string password { get; set; }
        public string token { get; set; }
    }
}