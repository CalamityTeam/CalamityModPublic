using CalamityMod.NPCs.PlaguebringerGoliath;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class PBGBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:PlaguebringerGoliath", IsSceneEffectActive(player));
        }
    }
}
