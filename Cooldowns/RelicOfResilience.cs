using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
	public class RelicOfResilience : CooldownHandler
    {
        public static new string ID => "RelicOfResilience";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Relic of Resilience Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/RelicOfResilience";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(122, 66, 59);
        public override Color CooldownEndColor => new Color(165, 103, 87);
    }
}