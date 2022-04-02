using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SphereBladed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bladed Sphere");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100);
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 25f)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                float num42 = 16f;
                float num43 = 3.2f;
                Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num44 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X;
                float num45 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y;
                float num46 = (float)Math.Sqrt((double)(num44 * num44 + num45 * num45));
                if (num46 > 3000f)
                {
                    projectile.Kill();
                }
                num46 = num42 / num46;
                num44 *= num46;
                num45 *= num46;
                if (projectile.velocity.X < num44)
                {
                    projectile.velocity.X = projectile.velocity.X + num43;
                    if (projectile.velocity.X < 0f && num44 > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + num43;
                    }
                }
                else if (projectile.velocity.X > num44)
                {
                    projectile.velocity.X = projectile.velocity.X - num43;
                    if (projectile.velocity.X > 0f && num44 < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - num43;
                    }
                }
                if (projectile.velocity.Y < num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num43;
                    if (projectile.velocity.Y < 0f && num45 > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num43;
                    }
                }
                else if (projectile.velocity.Y > num45)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num43;
                    if (projectile.velocity.Y > 0f && num45 < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num43;
                    }
                }
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value2))
                    {
                        projectile.Kill();
                    }
                }
            }
            projectile.rotation += 0.5f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.NPCHit34, projectile.position);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
