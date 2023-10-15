using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class GaelsRage : CooldownHandler
    {
        public static new string ID => "GaelsRage";

        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/GaelsRage";
        public override Color OutlineColor => new Color(217, 106, 161);
        public override Color CooldownStartColor => new Color(197, 134, 157);
        public override Color CooldownEndColor => new Color(238, 151, 184);
    }
}
