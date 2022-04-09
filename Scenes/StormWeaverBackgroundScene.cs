using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class StormWeaverBackgroundScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:StormWeaverFlash", IsSceneEffectActive(player));
        }
    }
}
