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

    private void CmdClearComponents() => ElectricalComponentManager.Instance.Cleanup();

    private void CmdClearWires() => ElectricalWireManager.Instance.Cleanup();

    private void CmdSpawnWire()
    {
        var player = GameManager.Instance.World.GetPrimaryPlayer();
        var start = player.GetLookRay().origin;
        var end = start + player.GetLookVector() * 3;

        var wire = new ElectricalWire();
        wire.Parent = GameManager.Instance.gameObject.transform;
        wire.AddSection(start, end);
    }

    private void CmdCollide()
    {
        var player = GameManager.Instance.World.GetPrimaryPlayer();
        var collider = RayCastUtils.GetLookedAtCollider(player, 4f);

        logger.Info($"collider: {collider?.gameObject?.name}");
    }

    private void CmdNodePosition(string[] args)
    {
        var player = GameManager.Instance.World.GetPrimaryPlayer();
        var nodeInstance = RayCastUtils.GetComponent<ElectricalNodeInstance>(player);

        if (nodeInstance == null)
        {
            logger.Error($"No looked nodeInstance");
            return;
        }

        nodeInstance.transform.localPosition = new Vector3(
            float.Parse(args[1]),
            float.Parse(args[2]),
            float.Parse(args[3])
        );
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

            case "clearwires":
            case "cw":
                CmdClearWires();
                break;

            case "wire":
                CmdSpawnWire();
                break;

            case "collide":
                CmdCollide();
                break;

            case "nodeposition":
            case "nodepos":
            case "np":
                CmdNodePosition(args);
                break;

            default:
                logger.Error($"Invalid command: '{_params[0]}'");
                break;
        }
    }
}