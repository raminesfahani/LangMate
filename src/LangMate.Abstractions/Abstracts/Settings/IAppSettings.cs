namespace LangMate.Abstractions.Abstracts.Settings
{
    interface IAppSettings
    {
        IGlobalSettings Global { get; set; }
        IWorkerSettings Worker { get; set; }
    }
}
