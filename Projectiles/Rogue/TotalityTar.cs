using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TotalityTar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Totality Tar");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
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
                    if (Math.Abs(projectile.velocity.X) < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
            if (projectile.velocity.Y < 0.25f && projectile.velocity.Y > 0.15f)
            {
                projectile.velocity.X *= 0.8f;
            }
            projectile.rotation = -projectile.velocity.X * 0.05f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.buffImmune[BuffID.Oiled])
            {
                target.buffImmune[BuffID.Oiled] = false;
            }
            target.AddBuff(BuffID.Oiled, 600);
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item74, projectile.position);
            Vector2 vector2 = new Vector2(20f, 20f);
            for (int index = 0; index < 3; ++index)
                Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 191, 0.0f, 0.0f, 0, Color.Red, 1f);
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 1.4f;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Dust dust1 = Main.dust[index2];
                dust1.noGravity = true;
                dust1.velocity *= 5f;
                int index3 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 3f;
            }
            int fireAmt = Main.rand.Next(2, 4);
            if (projectile.owner == Main.myPlayer)
            {
                for (int f = 0; f < fireAmt; f++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TotalityFire>(), projectile.damage, 1f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
