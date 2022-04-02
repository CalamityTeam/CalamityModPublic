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
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.velocity *= 0.95f;

            projectile.HealingProjectile(3, (int)projectile.ai[0], 6f, 15f, false);
            int dusty = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[dusty];
            dust.noGravity = true;
            dust.position.X -= projectile.velocity.X * 0.2f;
            dust.position.Y += projectile.velocity.Y * 0.2f;
        }
    }
}
