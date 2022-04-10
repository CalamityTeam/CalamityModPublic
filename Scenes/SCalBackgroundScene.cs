using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Skies;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SCalBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) || SCalSky.OverridingIntensity > 0f;

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:SupremeCalamitas", IsSceneEffectActive(player));
        }
    }
}
