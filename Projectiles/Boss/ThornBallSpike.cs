using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ThornBallSpike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        private const float MaxVelocity = 12f;
        private const int TimeLeft = 600;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = TimeLeft;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < MaxVelocity)
            {
                Projectile.velocity *= 1.035f;
                if (Projectile.velocity.Length() > MaxVelocity)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= MaxVelocity;
                }
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Poisoned, 90);

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            lightColor.A = (byte)Projectile.alpha;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
