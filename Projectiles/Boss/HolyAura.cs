using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyAura : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Aura");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            aiType = -1;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 210;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() / 2f;
            float time = Main.GlobalTime % 10f / 10f;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            int drawnAmt = 30;
            float[] posX = new float[drawnAmt];
            float[] posY = new float[drawnAmt];
            float[] hue = new float[drawnAmt];
            float[] size = new float[drawnAmt];
            int totalTime = 210;
            float colorChangeAmt = Utils.InverseLerp(0f, 60f, projectile.timeLeft, true) * Utils.InverseLerp(totalTime, totalTime - 60, projectile.timeLeft, true);
            float colorChangeAmt2 = Utils.InverseLerp(0f, 60f, projectile.timeLeft, true) * Utils.InverseLerp(totalTime, 90f, projectile.timeLeft, true);
            colorChangeAmt2 = Utils.InverseLerp(0.2f, 0.5f, colorChangeAmt2, true);
            float sizeScale = 0.8f;
            float sizeScalar = (1f - sizeScale) / drawnAmt;
            float yPosOffset = 60f;
            float xPosOffset = 400f;
            Vector2 scale = new Vector2(6f, 6f);

            for (int i = 0; i < drawnAmt; i++)
            {
                float timeScalar = (float)Math.Sin(time * MathHelper.TwoPi + (float)Math.PI / 2f + i / 2f);

                posX[i] = timeScalar * (xPosOffset - i * 3f);

                posY[i] = (float)Math.Sin(time * MathHelper.TwoPi * 2f + (float)Math.PI / 3f + i) * yPosOffset;
                posY[i] -= i * 3f;

                hue[i] = i / (float)drawnAmt * 2f + time;
                hue[i] = (timeScalar * 0.5f + 0.5f) * 0.6f + time;

                size[i] = sizeScale + (i + 1) * sizeScalar;
                size[i] *= 0.3f;

                Color color = Main.hslToRgb(hue[i] % 1f, 1f, 0.5f) * colorChangeAmt * colorChangeAmt2;

                bool underworld = projectile.ai[0] == 2f;
                if (Main.dayTime)
                {
                    color.R = 255;
                    if (underworld)
                        color.B = 0;
                }
                else
                {
                    color.B = 255;
                    if (underworld)
                        color.G = 0;
                    else
                        color.R = 0;
                }

                color.A /= 4;

                int fadeTime = 30;
                if (projectile.timeLeft < fadeTime)
                {
                    float amount2 = projectile.timeLeft / (float)fadeTime;

                    if (color.R > 0)
                        color.R = (byte)MathHelper.Lerp(0, color.R, amount2);
                    if (color.G > 0)
                        color.G = (byte)MathHelper.Lerp(0, color.G, amount2);
                    if (color.B > 0)
                        color.B = (byte)MathHelper.Lerp(0, color.B, amount2);

                    color.A = (byte)MathHelper.Lerp(0, color.A, amount2);
                }

                float rotation = MathHelper.PiOver2 + timeScalar * MathHelper.PiOver4 * -0.3f + (float)Math.PI * i;

                spriteBatch.Draw(texture, drawPosition + new Vector2(posX[i], posY[i]), null, color, rotation, origin, new Vector2(size[i], size[i]) * scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
