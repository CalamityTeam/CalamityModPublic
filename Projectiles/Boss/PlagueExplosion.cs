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
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.15f, 0f);

            float num461 = 25f;
            if (Projectile.ai[0] > 180f)
            {
                num461 -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (num461 <= 0f)
            {
                num461 = 0f;
                Projectile.Kill();
            }
            num461 *= 0.7f;

            Projectile.ai[0] += 4f;
            int num462 = 0;
            while (num462 < num461)
            {
                float num463 = Main.rand.Next(-7, 8) * Projectile.scale;
                float num464 = Main.rand.Next(-7, 8) * Projectile.scale;
                float num465 = Main.rand.Next(2, 6) * Projectile.scale;
                float num466 = (float)Math.Sqrt(num463 * num463 + num464 * num464);
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, 0f, 0f, 100, default);
                Main.dust[num467].noGravity = true;
                Main.dust[num467].position.X = Projectile.Center.X;
                Main.dust[num467].position.Y = Projectile.Center.Y;
                Main.dust[num467].position.X += Main.rand.Next(-10, 11);
                Main.dust[num467].position.Y += Main.rand.Next(-10, 11);
                Main.dust[num467].velocity.X = num463;
                Main.dust[num467].velocity.Y = num464;
                Main.dust[num467].scale = Projectile.scale * 0.35f;
                num462++;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
