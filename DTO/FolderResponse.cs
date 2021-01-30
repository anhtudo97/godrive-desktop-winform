using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GODrive.DTO
{
    public class Folder
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "path_display")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = ".tag")]
        public string Tag { get; set; }
    }
    class FolderData
    {
        [JsonProperty(PropertyName = "entries")]
        public Folder[] Entries { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }

        [JsonProperty(PropertyName = "has_more")]
        public bool HasMore { get; set; }
    }
    class FolderResponse
    {
        [JsonProperty(PropertyName = "data")]
        public FolderData Data { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }
    }
}
