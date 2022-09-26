using CalamityMod.NPCs.Yharon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class YharonBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Yharon>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Yharon", isActive);
        }
    }
}
