using System;

namespace LangMate.Abstractions.Abstracts.Persistence
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute(string collectionName) : Attribute
    {
        public string CollectionName { get; } = collectionName;
    }
}
