using Newtonsoft.Json;

namespace MarvelBot.Models.Responses
{
    public class Thumbnail
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        public string CreatePath(string format) =>
            $"{Path}/{format}.{Extension}";
    }
}