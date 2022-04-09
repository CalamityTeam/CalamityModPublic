using CalamityMod.Skies;
using Terraria;
using Terraria.Graphics.Effects;
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
            bool useExoMechs = IsSceneEffectActive(player);
            player.ManageSpecialBiomeVisuals("CalamityMod:ExoMechs", useExoMechs);
            if (useExoMechs)
                SkyManager.Instance.Activate("CalamityMod:ExoMechs", player.Center);
            else
                SkyManager.Instance.Deactivate("CalamityMod:ExoMechs");
        }
    }
}
