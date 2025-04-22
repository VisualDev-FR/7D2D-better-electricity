using System.Collections.Generic;

public class BetterElectricityConsoleCmd : ConsoleCmdAbstract
{
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

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        throw new System.NotImplementedException();
    }
}