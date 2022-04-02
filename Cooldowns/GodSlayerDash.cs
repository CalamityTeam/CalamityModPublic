using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class GodSlayerDash : CooldownHandler
    {
        public static new string ID => "GodSlayerDash";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "God Slayer Dash Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/GodSlayerDash";
        public override Color OutlineColor => Color.Lerp(new Color(173, 66, 203), new Color(252, 109, 202), instance.Completion);
        public override Color CooldownStartColor => new Color(252, 109, 202);
        public override Color CooldownEndColor => new Color(119, 254, 254);
    }
}
