using System.Collections.Generic;
using Newtonsoft.Json;

namespace MarvelBot.Models.Responses
{
    public class Data<T> where T: ItemBase
    {
        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }
}