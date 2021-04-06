using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_SEB_Eory
{
    public class User
    {

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Bio { get; set; }

        public string Image { get; set; }

        public string AuthToken { get; set; }

        public string ELO { get; set; }

        public string totalCount { get; set; }

        public string currentCount { get; set; }

        public string currentDuration { get; set; }

        public Library lib { get; set; }

        public User(string name)
        {
            this.Name = name;
        }

        public User()
        {
        }
    }
}
