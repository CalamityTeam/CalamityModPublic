using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class SandCloak : CooldownHandler
    {
        public static new string ID => "SandCloak";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/SandCloak";
        public override Color OutlineColor => new Color(209, 176, 114);
        public override Color CooldownStartColor => new Color(100, 64, 44);
        public override Color CooldownEndColor => new Color(132, 95, 54);
    }
}
