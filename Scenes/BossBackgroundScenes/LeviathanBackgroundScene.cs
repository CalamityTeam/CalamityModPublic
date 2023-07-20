using CalamityMod.NPCs.Leviathan;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class LeviathanBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => Main.zenithWorld ? NPC.AnyNPCs(ModContent.NPCType<Anahita>()) : NPC.AnyNPCs(ModContent.NPCType<Leviathan>());

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Leviathan", isActive);
        }
    }
}
