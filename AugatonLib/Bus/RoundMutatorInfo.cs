namespace AugatonLib.Bus
{
    public sealed class RoundMutatorInfo
    {
        public RoundMutatorInfo(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Key { get; }

        public string Name { get; }
    }
}
