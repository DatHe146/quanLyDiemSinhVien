using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class User
    {
        
        public string UserName { get; private set; }
        public string Password { get; set; } // Thay thế cho SetPassWord
        public string Role { get; private set; }

        public User(string userName, string passWord, string role)
        {
            UserName = userName;
            Password = passWord;
            Role = role;
        }
    }
}
