using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class UberBubble : ModProjectile
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
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.timeLeft = 30;
        }

        public override void AI()
        {
            projectile.velocity *= 0.975f;

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
                num745 -= -MathHelper.TwoPi;

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item96, projectile.position);
            int num190 = Main.rand.Next(4, 6);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(projectile.Center, 0, 0, 171, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int numBubbles = 0; numBubbles < 3; numBubbles++)
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * (Main.rand.NextFloat() * 2f), projectile.velocity.Y * (Main.rand.NextFloat() * 2f), ModContent.ProjectileType<BlueBubble>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}
