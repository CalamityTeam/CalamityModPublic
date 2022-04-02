using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class RainHeal : ModProjectile
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
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 165)
            {
                if (projectile.timeLeft > 150)
                    projectile.velocity *= 0.9f;
                else
                    projectile.HealingProjectile(8, projectile.owner, 12f, 15f, false);
            }

            float num498 = projectile.velocity.X * 0.2f;
            float num499 = -(projectile.velocity.Y * 0.2f);
            int num500 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Dust dust = Main.dust[num500];
            dust.noGravity = true;
            dust.position.X -= num498;
            dust.position.Y -= num499;
        }
    }
}
