using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class PermafrostConcoction : CooldownHandler
    {
        public static new string ID => "PermafrostConcoction";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/PermafrostConcoction";
        public override Color OutlineColor => new Color(0, 218, 255);
        public override Color CooldownStartColor => new Color(144, 184, 205);
        public override Color CooldownEndColor => new Color(232, 246, 254);
    }
}
