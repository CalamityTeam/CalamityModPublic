using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Exobeam : ModProjectile
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.light = 1f;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
                Main.PlaySound(SoundID.Item60, (int)projectile.position.X, (int)projectile.position.Y);
                projectile.localAI[1] += 1f;
            }
            counter++;
            if (counter == 12)
            {
                counter = 0;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 vector3 = Vector2.UnitX * (float)-(float)projectile.width / 2f;
                    vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    vector3 = vector3.RotatedBy((double)(projectile.rotation - 1.57079637f), default);
                    int num9 = Dust.NewDust(projectile.Center, 0, 0, 107, 0f, 0f, 160, new Color(0, 255, 255), 1f);
                    Main.dust[num9].scale = 1.1f;
                    Main.dust[num9].noGravity = true;
                    Main.dust[num9].position = projectile.Center + vector3;
                    Main.dust[num9].velocity = projectile.velocity * 0.1f;
                    Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
                }
            }
            projectile.alpha -= 40;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= 90f)
                {
                    projectile.localAI[0] = 0f;
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= 60f)
                {
                    projectile.localAI[0] = 0f;
                    projectile.ai[0] = 2f;
                    projectile.ai[1] = (float)Player.FindClosest(projectile.position, projectile.width, projectile.height);
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[0] == 2f)
            {
                Vector2 vector70 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                if (vector70.Length() < 30f)
                {
                    projectile.Kill();
                    return;
                }
                vector70.Normalize();
                vector70 *= 14f;
                vector70 = Vector2.Lerp(projectile.velocity, vector70, 0.6f);
                if (vector70.Y < 24f)
                {
                    vector70.Y = 24f;
                }
                float num804 = 0.4f;
                if (projectile.velocity.X < vector70.X)
                {
                    projectile.velocity.X = projectile.velocity.X + num804;
                    if (projectile.velocity.X < 0f && vector70.X > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + num804;
                    }
                }
                else if (projectile.velocity.X > vector70.X)
                {
                    projectile.velocity.X = projectile.velocity.X - num804;
                    if (projectile.velocity.X > 0f && vector70.X < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - num804;
                    }
                }
                if (projectile.velocity.Y < vector70.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num804;
                    if (projectile.velocity.Y < 0f && vector70.Y > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num804;
                    }
                }
                else if (projectile.velocity.Y > vector70.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num804;
                    if (projectile.velocity.Y > 0f && vector70.Y < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num804;
                    }
                }
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 192;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Zombie, (int)projectile.Center.X, (int)projectile.Center.Y, 103);
            for (int num193 = 0; num193 < 3; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int num194 = 0; num194 < 30; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
}
