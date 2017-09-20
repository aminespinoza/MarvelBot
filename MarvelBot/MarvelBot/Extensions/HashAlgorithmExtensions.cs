using System.Security.Cryptography;
using System.Text;

namespace MarvelBot.Extensions
{
    public static class HashAlgorithmExtensions
    {
        public static byte[] ComputeHash(this HashAlgorithm algorithm, string value) =>
            algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
    }
}