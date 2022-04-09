using CalamityMod.NPCs.Calamitas;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasCloneBackgroundScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<CalamitasRun3>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:CalamitasRun3", IsSceneEffectActive(player));
        }
    }
}
