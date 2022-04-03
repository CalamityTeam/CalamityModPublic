using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Rogue
{
    public class BettyExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Betty Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 75;
            Projectile.height = 75;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
                SpawnExplosionDust();
            if (Projectile.ai[0] <= 1f)
                Projectile.ai[0]++;
        }

        void SpawnExplosionDust()
        {
            // Sparks and such
            Vector2 corner = Projectile.position;
            for (int i = 0; i < 15; i++)
            {
                int idx = Dust.NewDust(corner, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 25; i++)
            {
                int idx = Dust.NewDust(corner, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(corner, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
            }

            // Smoke, which counts as a Gore
            CalamityUtils.ExplosionGores(Projectile.Center, 3);
        }

    }
}
