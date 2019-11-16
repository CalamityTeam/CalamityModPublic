using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Rogue
{
    public class BettyExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Betty Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 75;
            projectile.height = 75;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 20;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
                SpawnExplosionDust();
            if (projectile.ai[0] <= 1f)
                projectile.ai[0]++;
        }

        void SpawnExplosionDust()
        {
            // Sparks and such
            Vector2 corner = new Vector2(projectile.position.X, projectile.position.Y);
            for (int i = 0; i < 15; i++)
            {
                int idx = Dust.NewDust(corner, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 25; i++)
            {
                int idx = Dust.NewDust(corner, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(corner, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
            }

            // Smoke, which counts as a Gore
            Vector2 goreVec = new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f);
            for (int i = 0; i < 3; i++)
            {
                float smokeScale = 0.33f;
                if (i == 1)
                {
                    smokeScale = 0.66f;
                }
                if (i == 2)
                {
                    smokeScale = 1f;
                }
                int idx = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                Main.gore[idx].velocity *= smokeScale;
                Main.gore[idx].velocity.X += 1f;
                Main.gore[idx].velocity.Y += 1f;

                idx = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                Main.gore[idx].velocity *= smokeScale;
                Main.gore[idx].velocity.X -= 1f;
                Main.gore[idx].velocity.Y += 1f;

                idx = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                Main.gore[idx].velocity *= smokeScale;
                Main.gore[idx].velocity.X += 1f;
                Main.gore[idx].velocity.Y -= 1f;

                idx = Gore.NewGore(goreVec, default, Main.rand.Next(61, 64), 1f);
                Main.gore[idx].velocity *= smokeScale;
                Main.gore[idx].velocity.X -= 1f;
                Main.gore[idx].velocity.Y -= 1f;
            }
        }
    
    }
}
