using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class StormWeaverBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["CalamityMod:StormWeaverFlash"] != null && isActive != SkyManager.Instance["CalamityMod:StormWeaverFlash"].IsActive())
            {
                if (isActive)
                    SkyManager.Instance.Activate("CalamityMod:StormWeaverFlash", player.Center);
                else
                    SkyManager.Instance.Deactivate("CalamityMod:StormWeaverFlash");
            }
        }
    }
}
