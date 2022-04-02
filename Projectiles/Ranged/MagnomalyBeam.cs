using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MagnomalyBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 5;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1f)
            {
                projectile.ai[1] = 0f;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.25f / 255f, Main.DiscoG * 0.25f / 255f, Main.DiscoB * 0.25f / 255f);
            float num55 = 100f;
            float num56 = 2f; //3
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(100f, 2f, lightColor);

        public override Color? GetAlpha(Color lightColor) => new Color(0, 250, 75, projectile.alpha);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs();

        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs();
    }
}
