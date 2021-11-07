using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NanoPurgeLaser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nano Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0.7f, 0.1f);
            float num55 = 100f;
            float num56 = 3f;
            if (projectile.ai[1] == 0f)
            {
                projectile.localAI[0] += num56;
                if (projectile.localAI[0] > num55)
                {
                    projectile.localAI[0] = num55;
                }
            }
            else
            {
                projectile.localAI[0] -= num56;
                if (projectile.localAI[0] <= 0f)
                {
                    projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(96, 255, 96, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(40f, 2f, lightColor);

        public override void Kill(int timeLeft)
        {
            int dustID = 107;
            int dustAmt = Main.rand.Next(3, 7);
            Vector2 dustPos = projectile.Center - projectile.velocity / 2f;
            for (int i = 0; i < dustAmt; ++i)
            {
                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f, Scale: 0.8f);
                d.velocity *= 1.15f;
                d.noGravity = true;
            }
        }
    }
}
