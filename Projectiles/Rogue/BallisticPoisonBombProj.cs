using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BallisticPoisonBombProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 14, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.StickToTiles(true, false);
            if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
            {
                projectile.tileCollide = false;
                projectile.ai[1] = 0f;
                projectile.alpha = 255;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 128);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 10f)
            {
                projectile.ai[0] = 10f;
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
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 128);
            Main.PlaySound(SoundID.Item14, projectile.Center);
            int projAmt = Main.rand.Next(3, 5);
            if (projectile.owner == Main.myPlayer)
            {
                for (int s = 0; s < projAmt; s++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<BallisticPoisonBombSpike>(), (int)(projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                }
                int cloudAmt = Main.rand.Next(8, 13);
                for (int c = 0; c < cloudAmt; c++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<BallisticPoisonCloud>(), (int)(projectile.damage * 0.25), 1f, projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
                }
            }
            for (int d = 0; d < 5; d++)
            {
                int boom = Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 100, default, 2f);
                Main.dust[boom].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[boom].scale = 0.5f;
                    Main.dust[boom].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 9; d++)
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
            projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
            projectile.Kill();
        }
    }
}
