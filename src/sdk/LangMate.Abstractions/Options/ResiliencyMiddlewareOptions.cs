namespace LangMate.Abstractions.Options
{
    public class ResiliencyMiddlewareOptions
    {
        public int RetryCount { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 10;
        public int ExceptionsAllowedBeforeCircuitBreaking { get; set; } = 2;
        public long CircuitBreakingDurationSeconds { get; set; } = 30;
    }
}
