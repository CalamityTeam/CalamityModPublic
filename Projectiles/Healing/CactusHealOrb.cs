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
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.velocity.Y *= 0.98f;

            projectile.HealingProjectile(15, projectile.owner, 12f, 15f, false);
            int dusty = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(0, 200, 0), 1.5f);
            Dust dust = Main.dust[dusty];
            dust.noGravity = true;
            dust.position.X -= projectile.velocity.X * 0.2f;
            dust.position.Y += projectile.velocity.Y * 0.2f;
        }
    }
}
