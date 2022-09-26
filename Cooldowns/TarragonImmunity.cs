using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class TarragonImmunity : CooldownHandler
    {
        public static new string ID => "TarragonImmunity";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Tarragon Immunity Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/TarragonImmunity";
        public override Color OutlineColor => new Color(215, 182, 82);
        public override Color CooldownStartColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - instance.Completion);
    }
}
