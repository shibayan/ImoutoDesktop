using System.Threading.Tasks;

using ImoutoDesktop.Services;

namespace ImoutoDesktop.Commands;

public abstract class RemoteCommandBase : CommandBase
{
    protected RemoteCommandBase(string pattern, RemoteConnectionManager remoteConnectionManager)
        : base(pattern)
    {
        RemoteConnectionManager = remoteConnectionManager;
    }

    protected RemoteConnectionManager RemoteConnectionManager { get; }

    public override Task<CommandResult> PreExecute(string input)
    {
        if (RemoteConnectionManager.GetServiceClient() == null)
        {
            return Task.FromResult(new CommandResult { EventId = "Disconnect" });
        }

        return PreExecuteCore(input);
    }

    public override Task<CommandResult> Execute(string input)
    {
        if (RemoteConnectionManager.GetServiceClient() == null)
        {
            return Task.FromResult(new CommandResult { EventId = "Disconnect" });
        }

        return ExecuteCore(input);
    }

    protected virtual Task<CommandResult> PreExecuteCore(string input) => Task.FromResult(Succeeded());

    protected virtual Task<CommandResult> ExecuteCore(string input) => Task.FromResult(Succeeded());
}
