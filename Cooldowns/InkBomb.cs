using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class InkBomb : CooldownHandler
    {
        public static new string ID => "InkBomb";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/InkBomb";
        public override Color OutlineColor => new Color(205, 182, 137);
        public override Color CooldownStartColor => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), instance.Completion);
    }
}
