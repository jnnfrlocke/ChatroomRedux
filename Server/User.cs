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
            GenerateGuid();
            AddUser(userName);
            return userId.ToString();
        }

        public Guid GenerateGuid()
        {
            userId = Guid.NewGuid();
            return userId;
        }

        public void AddUser(string userName)
        {
            userDictionary.Add(userId, userName);
        }
    }
}
