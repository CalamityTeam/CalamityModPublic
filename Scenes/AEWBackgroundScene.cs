using CalamityMod.NPCs.AdultEidolonWyrm;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AEWBackgroundScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHeadHuge>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:AdultEidolonWyrm", IsSceneEffectActive(player));
        }
    }
}
