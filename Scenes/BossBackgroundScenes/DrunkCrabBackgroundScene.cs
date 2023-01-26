using CalamityMod.NPCs.Crabulon;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DrunkCrabBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Crabulon>()) && CalamityWorld.getFixedBoi;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DrunkCrabulon", isActive);
        }
    }
}
