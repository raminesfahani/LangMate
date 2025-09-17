namespace LangMate.Abstractions.Contracts.Settings
{
    interface IAppSettings
    {
        IGlobalSettings Global { get; set; }
        IWorkerSettings Worker { get; set; }
    }
}
