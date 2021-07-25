using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class BlueExoArrow : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Exo Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.arrow = true;
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
            float num55 = 40f;
            float num56 = 1.5f;
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

        public override Color? GetAlpha(Color lightColor) => new Color(0, 0, 250, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(40f, 1.5f, lightColor);

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
            OnHitEffects(target.Center);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            for (int x = 0; x < 3; x++)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(2), 500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<BlueExoArrow2>(), (int)(projectile.damage * 0.7), projectile.knockBack * 0.7f, projectile.owner);
                }
            }
        }
    }
}
