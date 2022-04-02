using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class Shockblast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 0.25f);
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
                float sizeMultiplier = (projectile.ai[1] * 0.5f) + 1f;
                float num463 = Main.rand.Next(-10, 11) * sizeMultiplier;
                float num464 = Main.rand.Next(-10, 11) * sizeMultiplier;
                float num465 = Main.rand.Next(3, 9) * sizeMultiplier;
                float num466 = (float)Math.Sqrt(num463 * num463 + num464 * num464);
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 185);
                Dust dust = Main.dust[num467];
                dust.scale = sizeMultiplier;
                dust.noGravity = true;
                dust.position.X = projectile.Center.X;
                dust.position.Y = projectile.Center.Y;
                dust.position.X += Main.rand.Next(-10, 11);
                dust.position.Y += Main.rand.Next(-10, 11);
                dust.velocity.X = num463;
                dust.velocity.Y = num464;
                num462++;
            }
        }
    }
}
