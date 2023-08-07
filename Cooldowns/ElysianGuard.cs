using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class ElysianGuard : CooldownHandler
    {
        public static new string ID => "ElysianGuard";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/ElysianGuard";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(181, 136, 177);
        public override Color CooldownEndColor => new Color(255, 194, 161);
    }
}
