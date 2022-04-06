using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 14, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            Projectile.StickToTiles(true, false);
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 128);
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Math.Abs(Projectile.velocity.X) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y += 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 128);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            int projAmt = Main.rand.Next(3, 5);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int s = 0; s < projAmt; s++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<BallisticPoisonBombSpike>(), (int)(Projectile.damage * 0.5), 0f, Projectile.owner, 0f, 0f);
                }
                int cloudAmt = Main.rand.Next(8, 13);
                for (int c = 0; c < cloudAmt; c++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<BallisticPoisonCloud>(), (int)(Projectile.damage * 0.25), 1f, Projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
                }
            }
            for (int d = 0; d < 5; d++)
            {
                int boom = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 100, default, 2f);
                Main.dust[boom].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[boom].scale = 0.5f;
                    Main.dust[boom].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 9; d++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(Projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
            Projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
            Projectile.Kill();
        }
    }
}
