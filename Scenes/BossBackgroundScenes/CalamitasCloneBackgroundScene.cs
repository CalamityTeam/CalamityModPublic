using CalamityMod.NPCs.Calamitas;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasCloneBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<CalamitasClone>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:CalamitasRun3", isActive);
        }
    }
}
