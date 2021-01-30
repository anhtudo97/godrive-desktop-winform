using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GODrive.DTO
{
    class ApiResponse
    {
        [JsonProperty(PropertyName = "entries")]
        public Folder[] Entries { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }

        [JsonProperty(PropertyName = "has_more")]
        public bool HasMore { get; set; }
    }
}
