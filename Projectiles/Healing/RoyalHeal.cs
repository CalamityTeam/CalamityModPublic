using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class RoyalHeal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

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
            for (int num493 = 0; num493 < 3; num493++)
            {
                float num494 = Projectile.velocity.X * 0.334f * (float)num493;
                float num495 = -(Projectile.velocity.Y * 0.334f) * (float)num493;
                int num496 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 20, 0f, 0f, 100, default, 1.1f);
                Dust dust = Main.dust[num496];
                dust.noGravity = true;
                dust.position.X -= num494;
                dust.position.Y -= num495;
            }
            for (int num497 = 0; num497 < 5; num497++)
            {
                float num498 = Projectile.velocity.X * 0.2f * (float)num497;
                float num499 = -(Projectile.velocity.Y * 0.2f) * (float)num497;
                int num500 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 20, 0f, 0f, 100, default, 1.3f);
                Dust dust2 = Main.dust[num500];
                dust2.noGravity = true;
                dust2.position.X -= num498;
                dust2.position.Y -= num499;
            }
        }
    }
}
