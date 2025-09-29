namespace LangMate.Abstractions.Abstracts.Settings
{
    public abstract class AppSettingsBase(IGlobalSettings global, IWorkerSettings worker) : IAppSettings
    {
        public IGlobalSettings Global { get; set; } = global;
        public IWorkerSettings Worker { get; set; } = worker;
    }
}
