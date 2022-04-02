using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class XerocHealOrb : ModProjectile
    {
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
            projectile.timeLeft = 240;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            projectile.HealingProjectile((int)projectile.ai[1], (int)projectile.ai[0], 5.5f, 15f);
            float num498 = projectile.velocity.X * 0.2f;
            float num499 = -(projectile.velocity.Y * 0.2f);
            if (projectile.timeLeft % 2 == 0)
            {
                int num500 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 62, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[num500];
                dust.noGravity = true;
                dust.position.X -= num498;
                dust.position.Y -= num499;
            }
        }
    }
}
