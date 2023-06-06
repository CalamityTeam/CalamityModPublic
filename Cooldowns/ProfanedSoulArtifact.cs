using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class ProfanedSoulArtifact : CooldownHandler
    {
        public static new string ID => "ProfanedSoulArtifact";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/ProfanedSoulArtifact";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(181, 136, 177);
        public override Color CooldownEndColor => new Color(255, 194, 161);
    }
}
