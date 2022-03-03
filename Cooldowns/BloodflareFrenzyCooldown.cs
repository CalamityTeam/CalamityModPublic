using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class BloodflareFrenzyCooldown : Cooldown
    {
        public override string ID => "BloodflareFrenzy";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Blood Frenzy Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BloodflareFrenzy";
        public override Color OutlineColor => new Color(229, 171, 124);
        public override Color CooldownStartColor => Color.Lerp(new Color(149, 127, 109), new Color(220, 101, 101), 1 - Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(149, 127, 109), new Color(220, 101, 101), 1 - Completion);

        public BloodflareFrenzyCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}