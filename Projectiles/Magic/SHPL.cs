using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SHPL : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
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
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.5f, 0.2f, 0.5f);
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
                    return;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, Main.DiscoG, 155, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(100f, 3f, lightColor);

        public override void Kill(int timeLeft)
        {
            int dustAmt = Main.rand.Next(3, 7);
            for (int d = 0; d < dustAmt; d++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    246,
                    73,
                    187
                });
                int idx = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, dustType, 0f, 0f, 100, default, 2.1f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }
    }
}
