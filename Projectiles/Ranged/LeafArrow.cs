using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class LeafArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.alpha -= 2;
            projectile.ai[0] = (float)Main.rand.Next(-100, 101) * 0.0025f;
            projectile.ai[1] = (float)Main.rand.Next(-100, 101) * 0.0025f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.05f;
                if ((double)projectile.scale > 1.2)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            else
            {
                projectile.scale -= 0.05f;
                if ((double)projectile.scale < 0.8)
                {
                    projectile.localAI[0] = 0f;
                }
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 3.14f;
            if (projectile.localAI[1] <= 30f)
            {
                projectile.localAI[1] += 1f;
                projectile.velocity.Y *= 0.975f;
                projectile.velocity.X *= 0.975f;
            }
            else if (projectile.localAI[1] <= 60f)
            {
                projectile.localAI[1] += 1f;
                projectile.velocity.Y *= 1.025f;
                projectile.velocity.X *= 1.025f;
            }
            else
            {
                projectile.localAI[1] = 0f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Grass, projectile.position);
            projectile.localAI[1] += 1f;
            for (int num407 = 0; num407 < 5; num407++)
            {
                int num408 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[num408].noGravity = true;
                Main.dust[num408].velocity *= 3f;
                Main.dust[num408].scale = 1.5f;
            }
        }
    }
}
