using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NorfleetComet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/GalacticaComet";

        private int noTileHitCounter = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int randomToSubtract = Main.rand.Next(1, 4);
            noTileHitCounter -= randomToSubtract;
            if (noTileHitCounter == 0)
            {
                projectile.tileCollide = true;
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, projectile.position);
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 18f)
            {
                projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustOffset = Vector2.UnitX * (float)-(float)projectile.width / 2f;
                    dustOffset += -Vector2.UnitY.RotatedBy((double)((float)l * MathHelper.Pi / 6f), default) * new Vector2(8f, 16f);
                    dustOffset = dustOffset.RotatedBy((double)(projectile.rotation - MathHelper.PiOver2), default);
                    int idx = Dust.NewDust(projectile.Center, 0, 0, Main.rand.NextBool(2) ? 221 : 244, 0f, 0f, 160, default, 1f);
                    Main.dust[idx].scale = 1.1f;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = projectile.Center + dustOffset;
                    Main.dust[idx].velocity = projectile.velocity * 0.1f;
                    Main.dust[idx].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[idx].position) * 1.25f;
                }
            }
            projectile.alpha -= 15;
            int alphaLimit = 150;
            if (projectile.alpha < alphaLimit)
            {
                projectile.alpha = alphaLimit;
            }
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Main.rand.NextBool(16))
            {
                Vector2 dustOffset = Vector2.UnitX.RotatedByRandom(Math.PI * 0.5).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(2) ? 221 : 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                Main.dust[idx].velocity = dustOffset * 0.66f;
                Main.dust[idx].position = projectile.Center + dustOffset * 12f;
            }
            if (projectile.ai[1] == 1f)
            {
                projectile.light = 0.5f;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(2) ? 221 : 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 100, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<NorfleetExplosion>(), (int)(projectile.damage * 0.3), projectile.knockBack * 0.1f, projectile.owner);
            }
            Main.PlaySound(SoundID.Item10, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 144);
            for (int d = 0; d < 4; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(2) ? 221 : 244, 0f, 0f, 50, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(2) ? 221 : 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(2) ? 221 : 244, 0f, 0f, 50, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft >= 600)
                return false;
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
