using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Cooldowns
{
    public class PrismaticLaser : CooldownHandler
    {
        public static new string ID => "PrismaticLaser";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/PrismaticLaser";
        public override Color OutlineColor => rainbowMode;
        public override Color CooldownStartColor => rainbowMode;
        public override Color CooldownEndColor => rainbowMode;

        internal Color rainbowMode => MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.3f % 1, new Color[] { new Color(103, 244, 251), new Color(255, 167, 236), new Color(255, 225, 136) });
    }
}
