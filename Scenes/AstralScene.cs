using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AstralScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneAstral;

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", IsSceneEffectActive(player));
        }
    }
}
