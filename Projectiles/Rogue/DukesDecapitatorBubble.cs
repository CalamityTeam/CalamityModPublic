using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DukesDecapitatorBubble : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/CoralBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.localAI[0] > 2f)
            {
                projectile.alpha -= 20;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }
            else
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[0] > 30f)
            {
                if (projectile.velocity.Y > -8f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                }
                projectile.velocity.X = projectile.velocity.X * 0.98f;
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation = projectile.velocity.X * 0.1f;
            if (projectile.wet)
            {
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                }
                if (projectile.velocity.Y > -8f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                }
                projectile.velocity.X = projectile.velocity.X * 0.94f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
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
