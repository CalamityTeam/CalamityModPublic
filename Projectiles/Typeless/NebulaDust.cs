using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class NebulaDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.Opacity = 0f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.02f;

            if (projectile.velocity.X < 0f)
                projectile.rotation -= Math.Abs(projectile.velocity.Y) * 0.02f;
            else
                projectile.rotation += Math.Abs(projectile.velocity.Y) * 0.02f;

            projectile.velocity *= 0.98f;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 60f)
            {
                if (projectile.Opacity > 0f)
                {
                    projectile.Opacity -= 0.02f;
                    if (projectile.Opacity < 0f)
                        projectile.Opacity = 0f;
                }
                else if (projectile.owner == Main.myPlayer)
                    projectile.Kill();
            }
            else if (projectile.Opacity < 0.7f)
            {
                projectile.Opacity += 0.1f;
                if (projectile.Opacity > 0.7f)
                    projectile.Opacity = 0.7f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);
            lightColor.G = (byte)(255 * projectile.Opacity);
            lightColor.B = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
