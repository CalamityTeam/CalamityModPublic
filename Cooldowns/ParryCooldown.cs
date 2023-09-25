using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class ParryCooldown : CooldownHandler
    {
        public static new string ID => "ParryCooldown";
        public override bool ShouldDisplay => true;
        public override bool SavedWithPlayer => false;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/ParryCooldown";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(181, 136, 177);
        public override Color CooldownEndColor => new Color(255, 194, 161);
    }
}
