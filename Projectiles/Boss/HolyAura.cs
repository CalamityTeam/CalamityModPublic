using System;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            AIType = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 210;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / 2f;
            float time = Main.GlobalTimeWrappedHourly % 10f / 10f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int drawnAmt = 30;
            float[] posX = new float[drawnAmt];
            float[] posY = new float[drawnAmt];
            float[] hue = new float[drawnAmt];
            float[] size = new float[drawnAmt];
            int totalTime = 210;
            float colorChangeAmt = Utils.GetLerpValue(0f, 60f, Projectile.timeLeft, true) * Utils.GetLerpValue(totalTime, totalTime - 60, Projectile.timeLeft, true);
            float colorChangeAmt2 = Utils.GetLerpValue(0f, 60f, Projectile.timeLeft, true) * Utils.GetLerpValue(totalTime, 90f, Projectile.timeLeft, true);
            colorChangeAmt2 = Utils.GetLerpValue(0.2f, 0.5f, colorChangeAmt2, true);
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

                bool underworld = Projectile.ai[0] == 2f;
                if (!Main.zenithWorld)
                {
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
                }

                color.A /= 4;

                int fadeTime = 30;
                if (Projectile.timeLeft < fadeTime)
                {
                    float amount2 = Projectile.timeLeft / (float)fadeTime;

                    if (color.R > 0)
                        color.R = (byte)MathHelper.Lerp(0, color.R, amount2);
                    if (color.G > 0)
                        color.G = (byte)MathHelper.Lerp(0, color.G, amount2);
                    if (color.B > 0)
                        color.B = (byte)MathHelper.Lerp(0, color.B, amount2);

                    color.A = (byte)MathHelper.Lerp(0, color.A, amount2);
                }

                float rotation = MathHelper.PiOver2 + timeScalar * MathHelper.PiOver4 * -0.3f + (float)Math.PI * i;

                Main.EntitySpriteDraw(texture, drawPosition + new Vector2(posX[i], posY[i]), null, color, rotation, origin, new Vector2(size[i], size[i]) * scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
