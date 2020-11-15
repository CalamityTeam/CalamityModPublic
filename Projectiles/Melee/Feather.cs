using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Feather : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/TradewindsProjectile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feather");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 150;
            projectile.aiStyle = 1;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }

        public override void Kill(int timeLeft)
        {
            int num3;
            for (int num611 = 0; num611 < 10; num611 = num3 + 1)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 64, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, default, 1f);
                num3 = num611;
            }
        }
    }
}
