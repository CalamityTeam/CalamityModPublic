using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BrimstoneFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0f);
            if (projectile.wet && !projectile.lavaWet)
            {
                projectile.Kill();
                if (projectile.owner == Main.myPlayer)
                {
                    int fireballAmt = Main.rand.Next(2, 4);
                    for (int j = 0; j < fireballAmt; j++)
                    {
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, -50f));
                        while (velocity.X == 0f)
                        {
                            velocity.X = (float)Main.rand.Next(-100, 101);
                        }
                        velocity.Normalize();
                        velocity *= Main.rand.NextFloat(70f, 100f) * 0.1f;
                        Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), velocity.X, velocity.Y, ModContent.ProjectileType<BrimstoneHomer>(), (int)(projectile.damage * 0.85), 0f, projectile.owner, 0f, 0f);
                    }
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                int brimstone = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                Main.dust[brimstone].noGravity = true;
                Main.dust[brimstone].velocity *= 0f;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 5f;
                if (projectile.velocity.Y == 0.0 && projectile.velocity.X != 0.0)
                {
                    projectile.velocity.X *= 0.97f;
                    if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
                projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.1f * projectile.direction;
            }
            if (projectile.velocity.Y <= 16f)
                return;
            projectile.velocity.Y = 16f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.velocity *= 0.8f;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
