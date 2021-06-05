using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class CoralBubbleSmall : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/CoralBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.scale = 0.5f;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.timeLeft = 360;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] > 2f)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }
            else
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[1] > 30f)
            {
                if (projectile.velocity.Y > -1.5f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                }
            }
            else
            {
                projectile.ai[1] += 1f;
            }
            if (projectile.wet)
            {
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                }
                if (projectile.velocity.Y > -1f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                }
            }
            int closestPlayer = (int)Player.FindClosest(projectile.Center, 1, 1);
            Vector2 distance = Main.player[closestPlayer].Center - projectile.Center;
            if (projectile.Distance(Main.player[closestPlayer].Center) < 7f)
            {
                Main.player[closestPlayer].AddBuff(BuffID.Gills, 30);
                projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            projectile.position = projectile.Center;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
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
