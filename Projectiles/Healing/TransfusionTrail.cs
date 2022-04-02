using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class TransfusionTrail : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.extraUpdates = 10;
        }

        public override void AI()
        {
            projectile.HealingProjectile((int)projectile.ai[1], (int)projectile.ai[0], 6.5f, 15f);
            float num494 = projectile.velocity.X * 0.334f;
            float num495 = -(projectile.velocity.Y * 0.334f);
            int num496 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 183, 0f, 0f, 100, default, 0.7f);
            Dust dust = Main.dust[num496];
            dust.noGravity = true;
            dust.position.X -= num494;
            dust.position.Y -= num495;
            for (int num497 = 0; num497 < 2; num497++)
            {
                float num498 = projectile.velocity.X * 0.2f * (float)num497;
                float num499 = -(projectile.velocity.Y * 0.2f) * (float)num497;
                int num500 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 183, 0f, 0f, 100, default, 0.9f);
                Dust dust2 = Main.dust[num500];
                dust2.noGravity = true;
                dust2.position.X -= num498;
                dust2.position.Y -= num499;
            }
        }
    }
}
