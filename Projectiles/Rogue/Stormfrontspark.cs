using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class Stormfrontspark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        //At first I thought about deleting em but then had an idea to give em some flair.
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.timeLeft = 5;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            int dusty = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
            Dust dust = Main.dust[dusty];
            dust.position.X -= 1f;
            dust.position.Y -= 1f;
            dust.scale += (float)Main.rand.Next(50) * 0.01f;
            dust.noGravity = true;
            dust.velocity.Y += 1f;
            if (Main.rand.NextBool())
            {
                int dusty2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(Main.rand.Next(20, 100), 204, 250), 1f);
                Dust dust2 = Main.dust[dusty2];
                dust2.position.X += 1f;
                dust2.position.Y -= 1f;
                dust2.scale += 0.2f + (float)Main.rand.Next(50) * 0.01f;
                dust2.noGravity = true;
                dust2.velocity *= 0.1f;
            }
            if ((double)Projectile.velocity.Y < 0.25 && (double)Projectile.velocity.Y > 0.15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.8f;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.velocity.Y += 0.15f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }
    }
}
