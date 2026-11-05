using System;
using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SCP500s.API
{
    public sealed class GlowService
    {
        private readonly PillRegistry registry;
        private readonly Dictionary<ushort, LightSourceToy> lights = new Dictionary<ushort, LightSourceToy>();

        public GlowService(PillRegistry registry) => this.registry = registry;

        private static Config Config => Plugin.Instance.Config;

        public int ActiveCount => lights.Count;

        // Evenements du jeu, et non ServerEvents.PickupCreated : LabAPI ne le declenche pas si le
        // wrapper existe deja au Start du pickup, ce qui est le cas apres Pickup.Create et apres
        // un depot, ou PlayerDroppedItemEventArgs cree le wrapper dans la meme frame.
        public void Start()
        {
            if (!Config.EnableGlow)
                return;

            ItemPickupBase.OnPickupAdded += OnPickupAdded;
            ItemPickupBase.OnPickupDestroyed += OnPickupDestroyed;
        }

        public void Stop()
        {
            ItemPickupBase.OnPickupAdded -= OnPickupAdded;
            ItemPickupBase.OnPickupDestroyed -= OnPickupDestroyed;

            ClearAll();
        }

        public void ClearAll()
        {
            foreach (KeyValuePair<ushort, LightSourceToy> entry in lights)
            {
                try
                {
                    Destroy(entry.Value);
                }
                catch (Exception e)
                {
                    Logger.Error($"GlowService.ClearAll: {e.Message}");
                }
            }

            lights.Clear();
        }

        private void OnPickupAdded(ItemPickupBase pickup)
        {
            try
            {
                if (pickup == null)
                    return;

                ushort serial = pickup.Info.Serial;

                if (lights.Count >= Config.MaxActiveGlows || lights.ContainsKey(serial))
                    return;

                if (!registry.TryGet(serial, out PillBase pill))
                    return;

                LightSourceToy light = LightSourceToy.Create(pickup.Position);

                light.Color = pill.GlowColor;
                light.Intensity = Config.GlowIntensity;
                light.Range = Config.GlowRange;
                light.ShadowType = LightShadows.None;
                light.Transform.SetParent(pickup.transform);

                lights[serial] = light;
            }
            catch (Exception e)
            {
                Logger.Error($"GlowService.OnPickupAdded: {e}");
            }
        }

        private void OnPickupDestroyed(ItemPickupBase pickup)
        {
            try
            {
                if (pickup == null || !lights.TryGetValue(pickup.Info.Serial, out LightSourceToy light))
                    return;

                Destroy(light);
                lights.Remove(pickup.Info.Serial);
            }
            catch (Exception e)
            {
                Logger.Error($"GlowService.OnPickupDestroyed: {e}");
            }
        }

        private static void Destroy(LightSourceToy light)
        {
            if (light is not null && !light.IsDestroyed)
                light.Destroy();
        }
    }
}
