using LangMate.Abstractions.Abstracts.Persistence;
using MongoDB.Bson;
using Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangMate.Abstractions.Abstracts.Documents
{
    [BsonCollection("conversations")]
    public class ConversationDocument : IDocument
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string ConversationId => Id.ToString();

        public string Model { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        
        public List<Message> Messages { get; set; } = [];

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
