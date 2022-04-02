using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MagnomalyRocket : ModProjectile
    {
        private bool spawnedAura = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuke");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 22;
            projectile.friendly = true;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //Lighting
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.25f / 255f, Main.DiscoG * 0.25f / 255f, Main.DiscoB * 0.25f / 255f);

            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 7)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;

            int dustType = Main.rand.NextBool(2) ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                dustType = 269;
            }
            if (projectile.owner == Main.myPlayer && !spawnedAura)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagnomalyAura>(), (int)(projectile.damage * 0.5f), projectile.knockBack * 0.5f, projectile.owner, projectile.identity, 0f);
                spawnedAura = true;
            }
            float dustOffsetX = projectile.velocity.X * 0.5f;
            float dustOffsetY = projectile.velocity.Y * 0.5f;
            if (Main.rand.NextBool(2))
            {
                int exo = Dust.NewDust(new Vector2(projectile.position.X + 3f + dustOffsetX, projectile.position.Y + 3f + dustOffsetY) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, dustType, 0f, 0f, 100, default, 0.5f);
                Main.dust[exo].scale *= (float)Main.rand.Next(10) * 0.1f;
                Main.dust[exo].velocity *= 0.2f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].noLight = true;
            }
            else
            {
                int exo = Dust.NewDust(new Vector2(projectile.position.X + 3f + dustOffsetX, projectile.position.Y + 3f + dustOffsetY) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, dustType, 0f, 0f, 100, default, 0.25f);
                Main.dust[exo].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[exo].velocity *= 0.05f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].noLight = true;
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 192);
                Main.PlaySound(SoundID.Item14, projectile.Center);
                //DO NOT REMOVE THIS PROJECTILE
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagnomalyExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);

                int dustType = Main.rand.NextBool(2) ? 107 : 234;
                if (Main.rand.NextBool(4))
                {
                    dustType = 269;
                }
                for (int d = 0; d < 30; d++)
                {
                    int exo = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1f);
                    Main.dust[exo].velocity *= 3f;
                    Main.dust[exo].noGravity = true;
                    Main.dust[exo].noLight = true;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[exo].scale = 0.5f;
                        Main.dust[exo].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 40; d++)
                {
                    int exo = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.5f);
                    Main.dust[exo].noGravity = true;
                    Main.dust[exo].noLight = true;
                    Main.dust[exo].velocity *= 5f;
                    exo = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.75f);
                    Main.dust[exo].velocity *= 2f;
                }
                CalamityUtils.ExplosionGores(projectile.Center, 9);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects();
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects();
            target.ExoDebuffs();
        }

        private void OnHitEffects()
        {
            if (projectile.owner == Main.myPlayer)
            {
                float random = Main.rand.Next(30, 90);
                float spread = random * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                for (int i = 0; i < 4; i++)
                {
                    double offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    int proj1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<MagnomalyBeam>(), projectile.damage / 4, projectile.knockBack / 4, projectile.owner, 0f, 1f);
                    int proj2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<MagnomalyBeam>(), projectile.damage / 4, projectile.knockBack / 4, projectile.owner, 0f, 1f);
                }
            }
        }
    }
}
