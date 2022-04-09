using CalamityMod.Skies;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ExoMechBackgroundScene : ModSceneEffect
    {
        public override int Music => Main.curMusic;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => ExoMechsSky.CanSkyBeActive;

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:ExoMechs", IsSceneEffectActive(player));
        }
    }
}
