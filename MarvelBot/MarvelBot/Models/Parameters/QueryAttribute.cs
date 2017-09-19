using System;

namespace MarvelBot.Models.Parameters
{
    public class QueryAttribute : Attribute
    {
        public string Name { get; }

        public QueryAttribute(string name) => Name = name;
    }
}