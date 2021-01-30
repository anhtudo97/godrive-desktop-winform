using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GODrive.DTO
{
    class UserModel
    {
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string id { get; set; }
        public string role { get; set; }
        public string token { get; set; }
        public string trelloId { get; set; }

        public static explicit operator UserModel(JObject v)
        {
            throw new NotImplementedException();
        }
    }
}
