using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using UnityEngine;

namespace BetterCoinflipsRewritten.Effects.Bad
{
    public sealed class TeleportToClassDCellsEffect : ICoinEffect
    {
        public string Name
        {
            get { return "TeleportToClassDCells"; }
        }

        public string Message
        {
            get
            {
                return Plugin.Instance.Translation
                    .TeleportToClassDCellsEffectMessage;
            }
        }

        public bool CanApply(Player player)
        {
            return player is not null &&
                   !player.IsDestroyed &&
                   player.IsAlive &&
                   !Decontamination.IsDecontaminating &&
                   FindCells() is not null;
        }

        public void Execute(Player player)
        {
            Room classDSpawn = FindCells();

            if (classDSpawn is null)
                return;

            player.Position = classDSpawn.Position + Vector3.up;
        }

        private static Room FindCells()
        {
            return Room.Get(RoomName.LczClassDSpawn).FirstOrDefault();
        }
    }
}
