namespace Replacer.API
{
    public readonly struct EffectSnapshot
    {
        public EffectSnapshot(string name, byte intensity, float duration)
        {
            Name = name;
            Intensity = intensity;
            Duration = duration;
        }

        public string Name { get; }

        public byte Intensity { get; }

        public float Duration { get; }
    }
}
