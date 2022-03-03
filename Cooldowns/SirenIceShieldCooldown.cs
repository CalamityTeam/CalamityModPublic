using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class SirenIceShieldCooldown : Cooldown
    {
        public override string SyncID => "SirenHeartShield";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Ice Shield Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SirenIceShield";
        public override Color OutlineColor => Color.Lerp(new Color(163, 186, 198), new Color(146, 187, 255), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownStartColor => new Color(124, 195, 214);
        public override Color CooldownEndColor => new Color(147, 230, 253);


        public SirenIceShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}