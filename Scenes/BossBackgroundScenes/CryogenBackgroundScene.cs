using CalamityMod.NPCs.Cryogen;
using CalamityMod.Skies;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CryogenBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player)
        {
            CryogenSky.UpdateDrawEligibility();
            bool result = NPC.AnyNPCs(ModContent.NPCType<Cryogen>()) || CryogenSky.ShouldDrawRegularly;
            CryogenSky.UpdateDrawEligibility();
            return result;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["CalamityMod:Cryogen"] != null && isActive != SkyManager.Instance["CalamityMod:Cryogen"].IsActive())
            {
                if (isActive)
                    SkyManager.Instance.Activate("CalamityMod:Cryogen", player.Center);
                else
                    SkyManager.Instance.Deactivate("CalamityMod:Cryogen");
            }
        }
    }
}
