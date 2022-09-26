using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class CactusHealOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity.Y *= 0.98f;

            Projectile.HealingProjectile(15, Projectile.owner, 12f, 15f, false);
            int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(0, 200, 0), 1.5f);
            Dust dust = Main.dust[dusty];
            dust.noGravity = true;
            dust.position.X -= Projectile.velocity.X * 0.2f;
            dust.position.Y += Projectile.velocity.Y * 0.2f;
        }
    }
}
