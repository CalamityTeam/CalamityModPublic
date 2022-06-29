using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AstralScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneAstral;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", isActive);
        }

        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";
    }
}
