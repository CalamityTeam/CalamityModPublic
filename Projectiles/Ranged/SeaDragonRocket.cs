using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SeaDragonRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("RPG");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 95;
            projectile.ranged = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item109, projectile.position);
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
            {
                projectile.tileCollide = false;
                projectile.ai[1] = 0f;
                projectile.alpha = 255;
                projectile.position.X = projectile.Center.X;
                projectile.position.Y = projectile.Center.Y;
                projectile.width = 200;
                projectile.height = 200;
                projectile.position.X -= (float)(projectile.width / 2);
                projectile.position.Y -= (float)(projectile.height / 2);
                projectile.knockBack = 10f;
            }
            else
            {
                if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        float num247 = 0f;
                        float num248 = 0f;
                        if (j == 1)
                        {
                            num247 = projectile.velocity.X * 0.5f;
                            num248 = projectile.velocity.Y * 0.5f;
                        }
                        int explosion = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, DustID.Fire, 0f, 0f, 100, default, 1f);
                        Main.dust[explosion].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                        Main.dust[explosion].velocity *= 0.2f;
                        Main.dust[explosion].noGravity = true;
                        explosion = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 33, 0f, 0f, 100, default, 0.5f);
                        Main.dust[explosion].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                        Main.dust[explosion].velocity *= 0.05f;
                    }
                }
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 192);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item110, projectile.position);
            for (int i = 0; i < 15; i++)
            {
                int smoke = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[smoke].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[smoke].scale = 0.5f;
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 25; j++)
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
            int projAmt = Main.rand.Next(2, 4);
            if (projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < projAmt; k++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<RocketFire>(), (int)(projectile.damage * 0.33), 0f, projectile.owner);
                }
            }
        }
    }
}
