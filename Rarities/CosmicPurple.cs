using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace CalamityMod.Rarities
{
    public class CosmicPurple : ModRarity
    {
        // Cosmic Purple is Rarity 14 (once was known as Dark Blue)
        public override Color RarityColor => TextClr * 2f;

        public static float MaxY = 4.5f;
        public static Color BloomClr = new Color(65, 38, 87, 0);
        public static Color TextClr = new Color(103, 66, 138, 255);
        public static UnifiedRandom rand = new UnifiedRandom(1);

        public sealed class CustomTextSnippet(string text) : TextSnippet
        {
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
            {
                size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

                if (color == default || color == Main.MouseTextColorReal)
                {
                    color = Colors.AlphaDarken(TextClr);
                }

                if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
                {
                    var font = FontAssets.MouseText.Value;
                    color.A = 255;
                    float pulsing = 2.5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
                    for (float f = 0f; f < MathHelper.TwoPi; f += 0.79f)
                    {
                        ChatManager.DrawColorCodedString(spriteBatch, font, text, position + new Vector2(pulsing, 0f).RotatedBy(f + Main.GlobalTimeWrappedHourly * 2f % MathHelper.TwoPi), color with { A = 0 } * 0.5f, 0, Vector2.Zero, new(scale));
                    }
                    ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, color * 2f, 0, Vector2.Zero, new(scale));
                    ChatManager.DrawColorCodedString(spriteBatch, font, text, position, Color.Black, 0, Vector2.Zero, new(scale));
                }
                return true;
            }
            public override float GetStringLength(DynamicSpriteFont font)
            {
                float size = font.MeasureString(text).X;
                return size * Scale;
            }
        }

        public static void Draw(Item Item, SpriteBatch spriteBatch, string text, int X, int Y, Color textColor, Color lightColor, float rotation,
            Vector2 origin, Vector2 baseScale, float time, bool renderTextSparkles, DynamicSpriteFont font)
        {
            var crystalTextGlow = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/CrystalTextGlow").Value;
            var sparkle = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/CrystalTextSparkle").Value;
            var fontSize = ChatManager.GetStringSize(font, text, new Vector2(1));
            var center = fontSize / 2f;
            if (Item.expert) textColor = Main.DiscoColor;

            // Get all snippets and convert all plain text snippets to the custom rarity snippet
            TextSnippet[] snippets = ChatManager.ParseMessage(text, textColor).ToArray();
            for (int i = 0; i < snippets.Length; i++)
            {
                TextSnippet textSnippet = snippets[i];
                if (snippets[i].GetType() == typeof(TextSnippet))
                {
                    snippets[i] = new CustomTextSnippet(textSnippet.Text);
                }
            }

            //Draw backglow
            var glowPosition = new Vector2(X + center.X, Y + center.Y / 1.5f);
            spriteBatch.Draw(crystalTextGlow, glowPosition, null, lightColor, rotation + MathHelper.PiOver2, new Vector2(6f, 33f),
               new Vector2(1.6f, fontSize.X / crystalTextGlow.Height * 1.2f), SpriteEffects.None, 0f);

            //Draw text
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new(X,Y), textColor, 0, Vector2.Zero, baseScale, out _, -1, true);

            //Draw sparkles
            if (!renderTextSparkles)
                return;


            rand.SetSeed(1);
            
            int sparkleCount = rand.Next((int)fontSize.X / 7, (int)fontSize.X / 5) + 1;
            var color2 = lightColor;
            color2.A = 0;
            var sparkleOrigin = new Vector2(15f, 15f);
            for (int i = 0; i < sparkleCount; i++)
            {
                var v = new Vector2(rand.NextFloat(fontSize.X), rand.NextFloat(fontSize.Y * 0.6f) + 1f);
                float lifeTime = Main.GlobalTimeWrappedHourly * 4f + rand.NextFloat(MathHelper.TwoPi);
                lifeTime %= MathHelper.TwoPi;

                if (lifeTime > MathHelper.TwoPi)
                    continue;

                float sinValue = (float)Math.Sin(lifeTime);
                var white = new Color(200 + lightColor.R / 20, 200 + lightColor.G / 20, 200 + lightColor.B / 20, 255) * sinValue;

                float sparkleRotationSpeed = Main.rand.NextFloat(0.8f, 1.5f); // Unique rotation rate per sparkle
                float sparkleRotation = time * sparkleRotationSpeed;

                spriteBatch.Draw(sparkle, new Vector2(X, Y - lifeTime * MaxY + 3f) + v, null, white, sparkleRotation, sparkleOrigin,
                    lifeTime / MathHelper.TwoPi * 0.3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkle, new Vector2(X, Y - lifeTime * MaxY + 2f) + v, null, white * 0.5f, sparkleRotation, sparkleOrigin,
                    lifeTime / MathHelper.TwoPi, SpriteEffects.None, 0f);

                var scale2 = (float)Math.Sin(lifeTime / MathHelper.PiOver2) + 1f;
                var scale3 = lifeTime / MathHelper.TwoPi;

                scale2 *= 0.2f;
                scale3 *= 0.15f;

                spriteBatch.Draw(sparkle, new Vector2(X, Y - lifeTime * MaxY + 2f) + v, null, color2 * sinValue, sparkleRotation, sparkleOrigin,
                    new Vector2(scale3, scale3) * 1.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkle, new Vector2(X, Y - lifeTime * MaxY + 2f) + v, null, color2 * sinValue, sparkleRotation, sparkleOrigin,
                    new Vector2(scale2, scale3), SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkle, new Vector2(X, Y - lifeTime * MaxY + 2f) + v, null, color2 * sinValue, sparkleRotation, sparkleOrigin,
                    new Vector2(scale3, scale2), SpriteEffects.None, 0f);
            }
        }

        public static void Draw(Item Item, string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale, Color? textColor = null, Color? lightColor = null, bool? renderTextSparkles = null)
        {
            Draw(Item, Main.spriteBatch, text, X, Y, Colors.AlphaDarken(textColor ?? TextClr), lightColor ?? BloomClr, rotation, origin, baseScale, Main.GlobalTimeWrappedHourly,
                renderTextSparkles ?? CalamityClientConfig.Instance.TextEffects, FontAssets.MouseText.Value);
        }

        public static void Draw(Item Item, DrawableTooltipLine line)
        {
            Draw(Item, line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale);
        }

        // TODO: Add a cooler alternative for reforge rarities
        /*public override int GetPrefixedRarity(int offset, float valueMult) => offset switch
        {
            -2 => ModContent.RarityType<Turquoise>(),
            -1 => ModContent.RarityType<PureGreen>(),
            1 => ModContent.RarityType<BurnishedAuric>(),
            2 => ModContent.RarityType<HotPink>(),
            _ => Type,
        };*/
    }
}
