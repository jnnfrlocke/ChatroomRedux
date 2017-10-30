using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class User
    {
        Dictionary<Guid, string> userDictionary = new Dictionary<Guid, string>();
        Guid userId;
        
        public string GenerateNewUser(string userName)
        {
            userId = Guid.NewGuid();
            userDictionary.Add(userId, userName);
            return userId.ToString();
        }



        //public User()
        //{

        //}


    }
}
