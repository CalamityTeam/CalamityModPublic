using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class SandTooth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Tooth");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            int num103 = Player.FindClosest(projectile.Center, 1, 1);
            projectile.ai[1] += 1f;
            if (projectile.ai[1] < 150f && projectile.ai[1] > 30f)
            {
                float inertia = 20f;
                float scaleFactor2 = projectile.velocity.Length();
                Vector2 vector11 = Main.player[num103].Center - projectile.Center;
                vector11.Normalize();
                vector11 *= scaleFactor2;
                projectile.velocity = (projectile.velocity * (inertia - 1f) + vector11) / inertia;
                projectile.velocity.Normalize();
                projectile.velocity *= scaleFactor2;
            }
            else if (projectile.ai[0] == 1f)
            {
                if (projectile.velocity.Length() < 16f)
                    projectile.velocity *= 1.01f;
            }

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver4;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
