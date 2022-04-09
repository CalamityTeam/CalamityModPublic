using CalamityMod.NPCs.Providence;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ProvBackgroundScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Providence>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Providence", IsSceneEffectActive(player));
        }
    }
}
