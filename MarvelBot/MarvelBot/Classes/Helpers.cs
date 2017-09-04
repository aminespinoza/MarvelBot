using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MarvelBot.Classes
{
    public static class Helpers
    {
        public static string FirstPathBuilder(string section, string queryValue, params string[] parameters)
        {
            const string privateKey = "tu llave privada";
            const string publicKey = "tu llave pública";
            const string timestamp = "9";
            string hash = HashGenerator(privateKey, publicKey, timestamp);

            StringBuilder queryBuilder = new StringBuilder("http://gateway.marvel.com/v1/public/");
            queryBuilder.Append(section + "?");
            queryBuilder.Append("apikey=" + publicKey);
            queryBuilder.Append("&ts=" + timestamp);
            queryBuilder.Append("&hash=" + hash);
            foreach (string parameter in parameters)
            {
                queryBuilder.Append("&" + parameter);
            }

            return queryBuilder.ToString();
        }

        private static string HashGenerator(string privateKey, string publicKey, string timestamp)
        {
            StringBuilder hash = new StringBuilder();
            string finalQuery = timestamp + privateKey + publicKey;

            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(finalQuery));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString();
        }

        public static string ImagePathBuilder(string path, string extension, string imageFormat)
        {
            StringBuilder imageBuilder = new StringBuilder();
            imageBuilder.Append(path);
            imageBuilder.Append("/" + imageFormat);
            imageBuilder.Append("." + extension);

            return imageBuilder.ToString();
        }
    }
}