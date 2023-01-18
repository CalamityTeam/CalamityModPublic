using CalamityMod.NPCs.Crabulon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DrunkCrabBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        //TODO -- Zenith seed.
        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Crabulon>()) && Main.getGoodWorld && Main.masterMode;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DrunkCrabulon", isActive);
        }
    }
}
