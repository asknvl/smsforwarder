using Newtonsoft.Json;
using smsforwarder.store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smsforwarder.users
{
    public class User
    {
        [JsonProperty]
        public long Id { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public bool NeedEmergencyNotify { get; set; } = true;
    }

    public class UserManager
    {
        #region vars
        IStorage<UserManager> storage;
        #endregion

        #region properties
        [JsonProperty]
        public List<User> users { get; set; } = new List<User>();
        #endregion        

        #region public
        public void Init()
        {
            storage = new Storage<UserManager>("users", "users.json", this);
            var t = storage.load();
            users = t.users;
        }

        public void Save()
        {
            storage.save(this);
        }

        public void Add(long id, string name)
        {
            if (!users.Any(u => u.Id == id))
            {
                users.Add(new User() { Id = id, Name = name });
                storage.save(this);
            }
        }

        public User? Get(long id)
        {
            return users.Find(u => u.Id == id);
        } 

        public bool Check(long id)
        {
            return users.Any(u => u.Id == id);
        }

        public List<long> GetIDs()
        {
            return users.Select(u => u.Id).ToList();
        }       

        public string GetInfo()
        {
            string res = "";

            foreach (var user in users)
            {
                res += $"{user.Id}:{user.Name}\n";
            }

            return res;
        }
        #endregion
    }
}
