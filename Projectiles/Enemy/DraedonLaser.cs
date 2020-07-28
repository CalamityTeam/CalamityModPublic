using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class DraedonLaser : ModProjectile
    {
        public float TrailLength
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const int MaxTrailPoints = 50;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 4;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item12, (int)projectile.Center.X, (int)projectile.Center.Y);
                projectile.localAI[0] = 1f;
            }
            projectile.alpha = (int)(Math.Sin(projectile.timeLeft / 240f * MathHelper.Pi) * 1.6f * 255f);
            if (projectile.alpha > 255)
                projectile.alpha = 255;
            TrailLength += 1.5f;
            if (TrailLength > MaxTrailPoints)
            {
                TrailLength = MaxTrailPoints;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 190, 255, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(Texture);
            float adjustedXOrigin = (texture.Width - projectile.width) * 0.5f + projectile.width * 0.5f;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Rectangle roughScreenBounds = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
            // Only draw when within the bounds of the screen for performance reasons.
            if (projectile.Hitbox.Intersects(roughScreenBounds))
            {
                Vector2 drawPosition = new Vector2(projectile.position.X + adjustedXOrigin, projectile.position.Y + projectile.height / 2) - Main.screenPosition;
                drawPosition += Vector2.UnitY * projectile.gfxOffY;
                for (int i = 1; i <= (int)TrailLength; i++)
                {
                    Vector2 drawOffset = Vector2.Normalize(projectile.velocity) * i * 1.5f;
                    Color drawColor = projectile.GetAlpha(lightColor); // The parameter doesn't really matter. The lightColor parameter in GetAlpha is ignored in this case.
                    drawColor.A = (byte)(255 - projectile.alpha);
                    drawColor *= (MaxTrailPoints - i) / (float)MaxTrailPoints;
                    float scale = MathHelper.Lerp(0.45f, 1f, 1f - i / TrailLength) * projectile.scale;
                    spriteBatch.Draw(texture, drawPosition - drawOffset, null, drawColor, projectile.rotation, new Vector2(adjustedXOrigin, projectile.height * 0.5f), scale, spriteEffects, 0f);
                }
            }
            return false;
        }
    }
}
