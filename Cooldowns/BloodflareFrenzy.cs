using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class BloodflareFrenzy : CooldownHandler
    {
        public static new string ID => "BloodflareFrenzy";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Blood Frenzy Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/BloodflareFrenzy";
        public override Color OutlineColor => new Color(229, 171, 124);
        public override Color CooldownStartColor => Color.Lerp(new Color(149, 127, 109), new Color(220, 101, 101), 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(149, 127, 109), new Color(220, 101, 101), 1 - instance.Completion);
    }
}