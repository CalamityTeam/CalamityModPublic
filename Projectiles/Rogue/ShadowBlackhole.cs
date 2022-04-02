using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShadowBlackhole : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blackhole");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 20;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }

        public override void AI()
        {
            // Update animation
            if (projectile.timeLeft % 5 == 0)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            projectile.ai[0]++;
            if (projectile.ai[0] > 120f)
            {
                projectile.scale *= 0.95f;
                projectile.Opacity *= 0.95f;
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, projectile.scale);
            }
            if (projectile.scale <= 0.05f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Blackout, 300);
        }
    }
}
