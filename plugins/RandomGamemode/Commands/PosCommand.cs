using AugatonLib.Commands;
using CommandSystem;

namespace RandomGamemode.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class PosCommand : PositionCommand
    {
        public PosCommand()
            : base("randomgamemode.manage")
        {
        }
    }
}
