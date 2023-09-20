using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AstralShot2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                Projectile.extraUpdates = 2;
                bool bossRush = BossRushEvent.BossRushActive;
                float maxVelocity = bossRush ? 3.75f : 3f;
                if (Projectile.velocity.Length() < maxVelocity)
                {
                    Projectile.velocity *= bossRush ? 1.02f : 1.015f;
                    if (Projectile.velocity.Length() > maxVelocity)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= maxVelocity;
                    }
                }
            }
            else
            {
                Vector2 targetLocation = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                if (Vector2.Distance(targetLocation, Projectile.Center) < 80f)
                    Projectile.tileCollide = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 45);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
