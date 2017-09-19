namespace MarvelBot.Models.Parameters
{
    public class Character : Parameter
    {
        public Character(string privateKey, string publicKey) : base(privateKey, publicKey)
        {
        }

        [Query("name")]
        public string Name { get; set; }
    }
}