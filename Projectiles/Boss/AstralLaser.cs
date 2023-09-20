using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AstralLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
                Projectile.frame = 0;

            // Speed up after some time
            if (Projectile.timeLeft < 600 && Main.expertMode)
            {
                if (Projectile.velocity.Length() < Projectile.ai[1])
                {
                    Projectile.velocity *= 1.005f;
                    if (Projectile.velocity.Length() > Projectile.ai[1])
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= Projectile.ai[1];
                    }
                }
            }

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            }

            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0f);

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 18f)
            {
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 3;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft >= 85;

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(Projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Projectile.timeLeft >= 85)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 45);
        }
    }
}
