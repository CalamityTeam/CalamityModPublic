using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class Stormfrontspark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spark");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.width = 6;
            projectile.height = 12;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.1f;
            int num199 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
            Dust dust = Main.dust[num199];
            dust.position.X -= 2f;
            dust.position.Y += 2f;
            dust.scale += (float)Main.rand.Next(50) * 0.01f;
            dust.noGravity = true;
            dust.velocity.Y -= 2f;
            if (Main.rand.NextBool(2))
            {
                int num200 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
                Dust dust2 = Main.dust[num200];
                dust2.position.X -= 2f;
                dust2.position.Y += 2f;
                dust2.scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                dust2.noGravity = true;
                dust2.velocity *= 0.1f;
            }
            if ((double)projectile.velocity.Y < 0.25 && (double)projectile.velocity.Y > 0.15)
            {
                projectile.velocity.X = projectile.velocity.X * 0.8f;
            }
            projectile.rotation = -projectile.velocity.X * 0.05f;
            projectile.velocity.Y += 0.15f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
