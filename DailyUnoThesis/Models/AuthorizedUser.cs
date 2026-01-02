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
            
        }
        public User AuthUser { get; set; }

        private static AuthorizedUser instance;
        public static AuthorizedUser GetInstance()
        {
            if(instance == null)
                instance = new AuthorizedUser();
            return instance;
        }
    }
}
