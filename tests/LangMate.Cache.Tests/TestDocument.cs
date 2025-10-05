using LangMate.Abstractions.Abstracts.Persistence;
using MongoDB.Bson;

namespace LangMate.Persistence.Tests
{
    [BsonCollection("test_documents")]
    public class TestDocument : IDocument
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public DateTime CreatedAt => throw new NotImplementedException();
    }
}