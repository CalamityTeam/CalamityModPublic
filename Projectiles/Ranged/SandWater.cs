using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SandWater : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 0.5f);
            for (int num457 = 0; num457 < 10; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.9f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.5f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item21, projectile.position);
            for (int dust = 0; dust <= 40; dust++)
            {
                float num463 = (float)Main.rand.Next(-10, 11);
                float num464 = (float)Main.rand.Next(-10, 11);
                float num465 = (float)Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default, 0.6f);
                Dust dust2 = Main.dust[num467];
                dust2.noGravity = true;
                dust2.position.X = projectile.Center.X;
                dust2.position.Y = projectile.Center.Y;
                dust2.position.X += (float)Main.rand.Next(-10, 11);
                dust2.position.Y += (float)Main.rand.Next(-10, 11);
                dust2.velocity.X = num463;
                dust2.velocity.Y = num464;
            }
        }
    }
}
