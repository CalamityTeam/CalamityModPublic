using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class TransfusionTrail : ModProjectile, ILocalizedModType
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
            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 6.5f, 15f);
            float dustX = Projectile.velocity.X * 0.334f;
            float dustY = -(Projectile.velocity.Y * 0.334f);
            int healRed = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 183, 0f, 0f, 100, default, 0.7f);
            Dust dust = Main.dust[healRed];
            dust.noGravity = true;
            dust.position.X -= dustX;
            dust.position.Y -= dustY;
            for (int i = 0; i < 2; i++)
            {
                float dustyX = Projectile.velocity.X * 0.2f * (float)i;
                float dustyY = -(Projectile.velocity.Y * 0.2f) * (float)i;
                int healRedMore = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 183, 0f, 0f, 100, default, 0.9f);
                Dust dust2 = Main.dust[healRedMore];
                dust2.noGravity = true;
                dust2.position.X -= dustyX;
                dust2.position.Y -= dustyY;
            }
        }
    }
}
