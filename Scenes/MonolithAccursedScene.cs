using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class MonolithAccursedScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override bool IsSceneEffectActive(Player player) => player.Calamity().monolithAccursedShader;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:MonolithAccursed", isActive);
        }
    }
}
