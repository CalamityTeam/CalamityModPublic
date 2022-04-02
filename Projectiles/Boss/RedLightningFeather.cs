using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class RedLightningFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Feather");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (float)Math.Atan2(-projectile.velocity.Y, -projectile.velocity.X);
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            }

            projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 1170) / 30f), 0f, 1f);

            Lighting.AddLight(projectile.Center, 0.7f * projectile.Opacity, 0f, 0f);

            projectile.ai[0] += 1f;
            float timeGateValue = 150f;
            float timeGateValue2 = 300f;
            if (projectile.ai[0] > timeGateValue)
            {
                int num103 = Player.FindClosest(projectile.Center, 1, 1);

                if (projectile.ai[0] <= timeGateValue2)
                {
                    if (projectile.ai[0] == timeGateValue2)
                    {
                        Vector2 aimVector = projectile.ai[1] == 0f ? (Main.player[num103].velocity * 20f) : Vector2.Zero;
                        Vector2 v4 = Main.player[num103].Center + aimVector - projectile.Center;

                        if (float.IsNaN(v4.X) || float.IsNaN(v4.Y))
                            v4 = -Vector2.UnitY;

                        EmitDust();

                        projectile.velocity = Vector2.Normalize(v4) * 18f;
                    }
                    else if (projectile.ai[0] > timeGateValue2 - 30f)
                    {
                        if (projectile.velocity.Length() > 2f)
                            projectile.velocity *= 0.9f;
                    }
                    else
                    {
                        if (projectile.velocity.Length() < 12f)
                            projectile.velocity *= 1.02f;

                        float scaleFactor2 = projectile.velocity.Length();
                        Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                        vector11.Normalize();
                        vector11 *= scaleFactor2;

                        projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= scaleFactor2;
                    }
                }
                else
                    projectile.tileCollide = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1, Main.projectileTexture[projectile.type], false);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            EmitDust();
            projectile.Damage();
        }

        private void EmitDust()
        {
            Main.PlaySound(SoundID.Item109, projectile.position);
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
            }
            for (int num194 = 0; num194 < 10; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Electrified, 60);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
