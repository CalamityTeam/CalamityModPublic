using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BeamStar : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 12;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 240;
            projectile.tileCollide = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            Time++;

            if (Time < 45f)
                projectile.velocity = projectile.velocity.RotatedBy((projectile.identity % 4 - 2) / 2f * 0.016f) * 1.004f;
            else
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(800f, false);
                if (potentialTarget != null)
                {
                    float destinationDeltaAngle = MathHelper.WrapAngle(projectile.AngleTo(potentialTarget.Center) - projectile.velocity.ToRotation());

                    // If the angle is <0, meaning it's to the left, the sine will return a negative number,
                    // and a positive number if >0, because WrapAngle constricts to angle to a bound of
                    // -pi and pi, which are where a sine's wave restarts anew.
                    float angularHomeDirection = Math.Sign(Math.Sin(destinationDeltaAngle));
                    projectile.velocity = projectile.velocity.RotatedBy(angularHomeDirection * 0.03f);
                }
            }

            if (Main.rand.NextBool(4))
            {
                Dust stardust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1f);
                stardust.position = projectile.Center;
                stardust.scale += Main.rand.NextFloat(0.5f);
                stardust.noGravity = true;
                stardust.velocity.Y -= 2f;
            }
            if (Main.rand.NextBool(6))
            {
                Dust stardust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 176, 0f, 0f, 100, default, 1f);
                stardust.position = projectile.Center;
                stardust.scale += Main.rand.NextFloat(0.3f, 1.1f);
                stardust.noGravity = true;
                stardust.velocity *= 0.1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition;
            Texture2D starTexture = Main.projectileTexture[projectile.type];
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                float scale = projectile.scale * MathHelper.Lerp(0.9f, 0.6f, i / (float)projectile.oldPos.Length) * 0.56f;
                drawPosition = projectile.oldPos[i] + projectile.Size * 0.5f - Main.screenPosition;
                spriteBatch.Draw(starTexture, drawPosition, null, Color.White, 0f, starTexture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }

            drawPosition = projectile.Center - Main.screenPosition;
            spriteBatch.Draw(starTexture, drawPosition, null, Color.White, 0f, starTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 176, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 180, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
