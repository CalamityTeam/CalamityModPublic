using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class OrangeExoArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.arrow = true;
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

        public override Color? GetAlpha(Color lightColor) => new Color(250, 100, 0, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(40f, 1.5f, lightColor);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.ExoDebuffs();

        public override void OnHitPvp(Player target, int damage, bool crit) => target.ExoDebuffs();

        public override void Kill(int timeLeft)
        {
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 188);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.Center);
            for (int d = 0; d < 4; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 50, default, 1f);
            }
            for (int d = 0; d < 40; d++)
            {
                int orange = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 0, default, 1.5f);
                Main.dust[orange].noGravity = true;
                Main.dust[orange].noLight = true;
                Main.dust[orange].velocity *= 3f;
                orange = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 50, default, 1f);
                Main.dust[orange].velocity *= 2f;
                Main.dust[orange].noGravity = true;
                Main.dust[orange].noLight = true;
            }
        }
    }
}
