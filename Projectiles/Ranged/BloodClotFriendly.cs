using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodClotFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Clot");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 2; num468++)
                {
                    Vector2 dspeed = -projectile.velocity * 0.7f;
                    int num469 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 5, dspeed.X, dspeed.Y, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                }
            }
        }
    }
}
