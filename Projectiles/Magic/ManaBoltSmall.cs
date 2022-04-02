using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ManaBoltSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
            projectile.magic = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 40 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            if (projectile.timeLeft < 40)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 450f, 6f, 20f);
            else
                projectile.velocity *= 0.975f;

            if (projectile.timeLeft % 4 == 0)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 15, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 15, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item10, projectile.position);
        }
    }
}
