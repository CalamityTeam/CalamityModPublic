using CalamityMod.NPCs.DevourerofGods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DoGBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>());

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHead", IsSceneEffectActive(player));
        }
    }
}
