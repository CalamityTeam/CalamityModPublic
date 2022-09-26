using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class BurntSiennaProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sienna");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            Projectile.HealingProjectile(3, (int)Projectile.ai[0], 6f, 15f, false);
            int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[dusty];
            dust.noGravity = true;
            dust.position.X -= Projectile.velocity.X * 0.2f;
            dust.position.Y += Projectile.velocity.Y * 0.2f;
        }
    }
}
