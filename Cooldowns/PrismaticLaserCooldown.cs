using Microsoft.Xna.Framework;
using Terraria;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Cooldowns
{
    public class PrismaticLaserCooldown : Cooldown
    {
        public override string SyncID => "PrismaticLaser";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Prismatic Laser Barrage Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PrismaticLaser";
        public override Color OutlineColor => rainbowMode;
        public override Color CooldownStartColor => rainbowMode;
        public override Color CooldownEndColor => rainbowMode;

        internal Color rainbowMode => MulticolorLerp(Main.GlobalTime * 0.3f % 1, new Color[] { new Color(103, 244, 251), new Color(255, 167, 236), new Color(255, 225, 136) });

        public PrismaticLaserCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}