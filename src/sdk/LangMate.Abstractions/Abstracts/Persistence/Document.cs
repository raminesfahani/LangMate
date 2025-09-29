using MongoDB.Bson;

namespace LangMate.Abstractions.Abstracts.Persistence
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
