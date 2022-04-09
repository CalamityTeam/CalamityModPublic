using CalamityMod.NPCs.Yharon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class YharonBackgroundScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Yharon>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Yharon", IsSceneEffectActive(player));
        }
    }
}
