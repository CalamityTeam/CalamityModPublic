using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VenusianBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + MathHelper.ToRadians(45);
            Lighting.AddLight(projectile.Center, 0.25f, 0.2f, 0f);
            if (projectile.wet && !projectile.lavaWet)
            {
                projectile.Kill();
            }
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item73, projectile.position);
                projectile.localAI[0] += 1f;
            }
            for (int num457 = 0; num457 < 3; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 100, default, 1.2f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.5f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int explosionDamage = projectile.damage;
                float explosionKB = 6f;
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<VenusianExplosion>(), explosionDamage, explosionKB, projectile.owner);

                int cinderDamage = (int)(projectile.damage * 0.75);
                float cinderKB = 0f;
                Vector2 cinderPos = projectile.oldPosition + 0.5f * projectile.Size;
                int numCinders = Main.rand.Next(7, 10);
                for (int i = 0; i < numCinders; i++)
                {
                    Vector2 cinderVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (cinderVel.X == 0f && cinderVel.Y == 0f)
                    {
                        cinderVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    cinderVel.Normalize();
                    cinderVel *= Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(cinderPos, cinderVel, ModContent.ProjectileType<VenusianFlame>(), cinderDamage, cinderKB, projectile.owner);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
