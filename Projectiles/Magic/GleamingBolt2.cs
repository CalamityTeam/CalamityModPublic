using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GleamingBolt2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 90 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            if (projectile.timeLeft < 90)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 8f, 20f);
            else
                projectile.velocity *= 0.985f;

            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                64,
                204
            });
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int randomDust = Main.rand.Next(2);
                if (randomDust == 0)
                {
                    randomDust = 64;
                }
                else
                {
                    randomDust = 204;
                }
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
