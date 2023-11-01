using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class RoyalHeal : ModProjectile, ILocalizedModType
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
        }

        public override void AI()
        {
            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 5.5f, 15f);
            for (int i = 0; i < 3; i++)
            {
                float dustX = Projectile.velocity.X * 0.334f * (float)i;
                float dustY = -(Projectile.velocity.Y * 0.334f) * (float)i;
                int pureHeal = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 20, 0f, 0f, 100, default, 1.1f);
                Dust dust = Main.dust[pureHeal];
                dust.noGravity = true;
                dust.position.X -= dustX;
                dust.position.Y -= dustY;
            }
            for (int j = 0; j < 5; j++)
            {
                float dustX2 = Projectile.velocity.X * 0.2f * (float)j;
                float dustY2 = -(Projectile.velocity.Y * 0.2f) * (float)j;
                int pureHeal2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 20, 0f, 0f, 100, default, 1.3f);
                Dust dust2 = Main.dust[pureHeal2];
                dust2.noGravity = true;
                dust2.position.X -= dustX2;
                dust2.position.Y -= dustY2;
            }
        }
    }
}
