using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class EarthHealOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            projectile.HealingProjectile((int)projectile.ai[1], (int)projectile.ai[0], 8f, 15f);
            int dusty = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 74, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[dusty];
            dust.noGravity = true;
            dust.position.X -= projectile.velocity.X * 0.2f;
            dust.position.Y += projectile.velocity.Y * 0.2f;
        }
    }
}
