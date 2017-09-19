using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MarvelBot.Models.Parameters
{
    //https://developer.marvel.com/documentation/authorization
    public class Parameter
    {
        private readonly string _privateKey;
        private readonly string _publicKey;

        public Parameter(string privateKey, string publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        //{section}?apikey={publicKey}&ts={timestamp}&hash={hash}
        [Query("apikey")]
        public string ApiKey => _publicKey;

        [Query("ts")]
        public int TimeStamp { get; set; }

        [Query("hash")]
        public string Hash => 
            new MD5CryptoServiceProvider()
                .ComputeHash(new UTF8Encoding().GetBytes(String.Join(String.Empty, TimeStamp, _privateKey, _publicKey)))
                .Select(b => b.ToString("x2"))
                .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s))
                .ToString();

        public override string ToString()
        {
            var query = HttpUtility.ParseQueryString(String.Empty);
            //https://docs.microsoft.com/en-us/dotnet/standard/attributes/retrieving-information-stored-in-attributes
            //https://stackoverflow.com/a/6637710/294804
            GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { 
                    Name = p.GetCustomAttributes(true).OfType<QueryAttribute>().FirstOrDefault()?.Name ?? String.Empty,
                    Value = p.GetValue(this).ToString()
                })
                .Where(x => !String.IsNullOrEmpty(x.Value))
                .ToList().ForEach(x => query.Add(x.Name, x.Value));
            return query.ToString();
        }
    }
}