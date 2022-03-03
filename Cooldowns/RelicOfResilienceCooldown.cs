using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class RelicOfResilienceCooldown : Cooldown
    {
        public override string SyncID => "RelicOfResilience";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Relic of Resilience Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/RelicOfResilience";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(122, 66, 59);
        public override Color CooldownEndColor => new Color(165, 103, 87);

        public RelicOfResilienceCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}