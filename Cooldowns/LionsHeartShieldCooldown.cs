using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class LionHeartShieldCooldown : Cooldown
    {
        public override string SyncID => "LionHeartShield";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Energy Shell Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/LionHeartShield";
        public override Color OutlineColor => new Color(232, 239, 239);
        public override Color CooldownStartColor => new Color(17, 242, 244);
        public override Color CooldownEndColor => Color.White;

        public LionHeartShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}