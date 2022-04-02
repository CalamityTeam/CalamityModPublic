using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TotalityFire : ModProjectile
    {
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //make it face the way it's going
            if (projectile.ai[1] > 0f)
            {
                projectile.rotation = -projectile.velocity.X * 0.05f + MathHelper.PiOver2;
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation();
            }
            projectile.ai[1]--;

            //frames
            if (!initialized)
            {
                initialized = true;
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            //movement
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X *= -0.1f;
            }
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X *= -0.5f;
            }
            if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
            {
                projectile.velocity.Y *= -0.5f;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 5f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X *= 0.97f;
                    if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }
            if (projectile.velocity.Y < 0.25f && projectile.velocity.Y > 0.15f)
            {
                projectile.velocity.X *= 0.8f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            //dust
            if (Main.rand.NextBool(4))
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.position.X -= 2f;
                dust.position.Y += 2f;
                dust.scale += (float)Main.rand.Next(50) * 0.01f;
                dust.noGravity = true;
                dust.velocity.Y -= 2f;
            }
            if (Main.rand.NextBool(10))
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[fire];
                dust.position.X -= 2f;
                dust.position.Y += 2f;
                dust.scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[1] = 10f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
