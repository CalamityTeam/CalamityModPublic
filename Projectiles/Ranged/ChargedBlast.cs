using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ChargedBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
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
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0.3f, 0.7f);
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

        public override Color? GetAlpha(Color lightColor) => new Color(100, 100, 255, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(100f, 3f, lightColor);

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item62, (int)projectile.position.X, (int)projectile.position.Y);
            int projectiles = Main.rand.Next(2, 5);
            if (projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < projectiles; k++)
                {
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-10, 11) * 2f, (float)Main.rand.Next(-10, 11) * 2f, ModContent.ProjectileType<ChargedBlast2>(),
                    (int)((double)projectile.damage * 0.85), (float)(int)((double)projectile.knockBack * 0.5), Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
