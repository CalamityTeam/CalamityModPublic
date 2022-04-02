using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Magic
{
    public class BlueBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.timeLeft = 120;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 90 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.velocity *= 0.99f;

            projectile.scale += 0.005f;

            if (projectile.alpha > 0)
                projectile.alpha -= 30;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            Vector2 v2 = projectile.ai[0].ToRotationVector2();
            float num743 = projectile.velocity.ToRotation();
            float num744 = v2.ToRotation();
            double num745 = num744 - num743;
            if (num745 > MathHelper.Pi)
                num745 -= MathHelper.TwoPi;
            if (num745 < -MathHelper.Pi)
                num745 += MathHelper.TwoPi;

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (projectile.timeLeft < 90)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 400f, 8f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.Center);
            int num190 = Main.rand.Next(5, 9);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(projectile.Center, 0, 0, 206, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
        }
    }
}
