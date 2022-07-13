using CalamityMod.NPCs.AdultEidolonWyrm;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AEWBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<AdultEidolonWyrmHead>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:AdultEidolonWyrm", isActive);
        }
    }
}
