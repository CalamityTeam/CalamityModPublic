using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresPlasmaBolt : ModProjectile
    {
        private const int timeLeft = 360;

        private const float maxVelocity = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exoplasma Bolt");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = timeLeft;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            // 22 frames starting at 0.5
            // 17 frames starting at 1
            if (projectile.velocity.Length() < maxVelocity)
            {
                projectile.velocity *= 1.075f;
                if (projectile.velocity.Length() > maxVelocity)
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= maxVelocity;
                }
            }

            int fadeOutTime = 15;
            int fadeInTime = 3;
            if (projectile.timeLeft < fadeOutTime)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / (float)fadeOutTime, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - (timeLeft - fadeInTime)) / (float)fadeInTime), 0f, 1f);

            Lighting.AddLight(projectile.Center, 0f, 0.4f * projectile.Opacity, 0f);

            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.CursedInferno, 90);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);
            lightColor.G = (byte)(255 * projectile.Opacity);
            lightColor.B = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
