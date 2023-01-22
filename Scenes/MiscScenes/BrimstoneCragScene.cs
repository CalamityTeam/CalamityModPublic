using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class BrimstoneCragScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneCalamity;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:BrimstoneCrag", isActive);
        }
    }
}
