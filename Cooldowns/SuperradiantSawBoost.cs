using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class SuperradiantSawBoost : CooldownHandler
    {
        public static new string ID => "SuperradiantSawBoost";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/SuperradiantSawBoost";
        public override Color OutlineColor => new Color(207, 207, 207);
        public override Color CooldownStartColor => new Color(122, 240, 58);
        public override Color CooldownEndColor => new Color(32, 186, 171);
    }
}
