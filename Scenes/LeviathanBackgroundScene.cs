using CalamityMod.NPCs.Leviathan;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class LeviathanBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Leviathan>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Leviathan", IsSceneEffectActive(player));
        }
    }
}
