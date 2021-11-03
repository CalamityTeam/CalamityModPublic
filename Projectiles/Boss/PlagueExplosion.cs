using CalamityMod.Buffs.DamageOverTime;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class PlagueExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Stinger Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
			projectile.scale = 1.5f;
			projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.15f, 0f);
            bool flag15 = false;
            bool flag16 = false;
            if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
            {
                flag15 = true;
            }
            if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
            {
                flag15 = true;
            }
            if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
            {
                flag16 = true;
            }
            if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
            {
                flag16 = true;
            }
            if (flag15 && flag16)
            {
                projectile.Kill();
            }
            float num461 = 25f;
            if (projectile.ai[0] > 180f)
            {
                num461 -= (projectile.ai[0] - 180f) / 2f;
            }
            if (num461 <= 0f)
            {
                num461 = 0f;
                projectile.Kill();
            }
            num461 *= 0.7f;
            projectile.ai[0] += 4f;
            int num462 = 0;
            while (num462 < num461)
            {
                float num463 = Main.rand.Next(-10, 11);
                float num464 = Main.rand.Next(-10, 11);
                float num465 = Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt(num463 * num463 + num464 * num464);
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0f, 0f, 100, default, 0.5f);
                Main.dust[num467].noGravity = true;
                Main.dust[num467].position.X = projectile.Center.X;
                Main.dust[num467].position.Y = projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[num467];
                expr_149DF_cp_0.position.X += Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[num467];
                expr_14A09_cp_0.position.Y += Main.rand.Next(-10, 11);
                Main.dust[num467].velocity.X = num463;
                Main.dust[num467].velocity.Y = num464;
                num462++;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
