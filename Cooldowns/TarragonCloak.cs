using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class TarragonCloak : CooldownHandler
    {
        public static new string ID => "TarragonCloak";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Tarragon Cloak Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/TarragonCloak";
        public override Color OutlineColor => new Color(158, 204, 173);
        public override Color CooldownStartColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - instance.Completion);
    }
}