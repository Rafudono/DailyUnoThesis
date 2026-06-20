using DailyUnoThesis.Models.MainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyUnoThesis.Models
{
    public class AuthorizedUser
    {
        public AuthorizedUser()
        {
            instance = this;
        }
        public User AuthUser { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        private static AuthorizedUser instance;
        public static AuthorizedUser GetInstance()
        {
            if(instance == null)
                instance = new AuthorizedUser();
            return instance;
        }
    }
}
