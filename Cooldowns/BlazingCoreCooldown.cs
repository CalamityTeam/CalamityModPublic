using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class BlazingCoreCooldown : CooldownHandler
    {
        public static new string ID => "BlazingCore";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/BlazingCore";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(181, 136, 177);
        public override Color CooldownEndColor => new Color(255, 194, 161);
    }
}
