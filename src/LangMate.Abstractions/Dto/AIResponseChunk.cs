namespace LangMate.Abstractions.Dto
{
    public class AIResponseChunk : AIResponse
    {
        public int? Sequence { get; set; }
        public required bool IsFinalChunk { get; set; }
    }
}
