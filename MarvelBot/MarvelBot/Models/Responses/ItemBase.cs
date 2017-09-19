using Newtonsoft.Json;

namespace MarvelBot.Models.Responses
{
    public class ItemBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}