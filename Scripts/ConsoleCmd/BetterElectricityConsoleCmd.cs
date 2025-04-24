using System.Collections.Generic;

public class BetterElectricityConsoleCmd : ConsoleCmdAbstract
{
    private static readonly Logging.Logger logger = Logging.CreateLogger<BetterElectricityConsoleCmd>();

    public override string[] getCommands()
    {
        return new string[] { "belec" };
    }

    public override string getDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string getHelp()
    {
        return base.getHelp();
    }

    private void CmdClearComponents()
    {
        ElectricalComponentManager.Instance.Cleanup();
    }

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        var args = _params.ToArray();

        if (args.Length == 0)
        {
            Log.Out(getHelp());
            return;
        }

        switch (args[0].ToLower())
        {
            case "clearcomponents":
            case "cc":
                CmdClearComponents();
                break;

            default:
                logger.Error($"Invalid or not implemented command: '{_params[0]}'");
                break;
        }
    }
}