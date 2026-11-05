namespace AugatonLib.Bus
{
    public sealed class StaffAction
    {
        public StaffAction(string plugin, string author, string action)
        {
            Plugin = plugin;
            Author = author;
            Action = action;
        }

        public string Plugin { get; }

        public string Author { get; }

        public string Action { get; }

        public override string ToString() => $"[{Plugin}] {Author} {Action}";
    }
}
