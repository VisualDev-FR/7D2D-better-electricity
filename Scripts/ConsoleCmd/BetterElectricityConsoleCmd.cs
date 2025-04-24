using System.Collections.Generic;
using UnityEngine;

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

    private void CmdSpawnWire(string[] args)
    {
        var parent = GameManager.Instance.gameObject.transform;
        var player = GameManager.Instance.World.GetPrimaryPlayer();
        var start = player.GetLookRay().origin;
        var end = start + player.GetLookVector() * 3;

        WireBuilder.BuildWire(parent, start, end);
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

            case "wire":
                CmdSpawnWire(args);
                break;

            default:
                logger.Error($"Invalid command: '{_params[0]}'");
                break;
        }
    }
}