using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class FungalHeal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 5f, 15f);
            float dustX = Projectile.velocity.X * 0.334f;
            float dustY = -(Projectile.velocity.Y * 0.334f);
            int fungi = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 0.5f);
            Dust dust = Main.dust[fungi];
            dust.noGravity = true;
            dust.position.X -= dustX;
            dust.position.Y -= dustY;
            float dustX2 = Projectile.velocity.X * 0.2f;
            float dustY2 = -(Projectile.velocity.Y * 0.2f);
            int fungi2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 56, 0f, 0f, 100, default, 0.7f);
            Dust dust2 = Main.dust[fungi2];
            dust2.noGravity = true;
            dust2.position.X -= dustX2;
            dust2.position.Y -= dustY2;
        }
    }
}
