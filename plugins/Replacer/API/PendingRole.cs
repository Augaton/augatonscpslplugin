namespace Replacer.API
{
    public sealed class PendingRole
    {
        public PendingRole(PlayerSnapshot snapshot, float expiresAt)
        {
            Snapshot = snapshot;
            ExpiresAt = expiresAt;
        }

        public PlayerSnapshot Snapshot { get; }

        public float ExpiresAt { get; }
    }
}
