using System.Net;
using Newtonsoft.Json;

namespace MarvelBot.Models.Responses
{
    public class Response<T> where T: ItemBase
    {
        [JsonProperty("code")]
        public HttpStatusCode Code { get; set; }

        [JsonProperty("data")]
        public Data<T> Data { get; set; }
    }
}