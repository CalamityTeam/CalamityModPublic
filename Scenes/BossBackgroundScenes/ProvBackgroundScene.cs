using CalamityMod.NPCs.Providence;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ProvBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Providence>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Providence", isActive);
        }
    }
}
