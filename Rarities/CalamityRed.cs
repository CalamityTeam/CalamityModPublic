using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.Rarities
{
    public class CalamityRed : ModRarity
    {
        // Calamity Red is Rarity 17
        public override Color RarityColor => TextClr * 2f;

        public static float MaxY = 4.5f;
        public static Color BloomClr = new Color(180, 20, 75, 0);
        public static Color TextClr = new Color(242, 27, 27, 255);

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
                    var time = Main.GlobalTimeWrappedHourly;

                    var crystalTextGlow = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/CrystalTextGlow").Value;
                    var sparkle = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/CrystalTextSparkle").Value;
                    var fontSize = ChatManager.GetStringSize(font, text, new Vector2(1));
                    var center = fontSize / 2f;

                    var glowPosition = position + new Vector2(size.X * 0.5f, size.Y * 0.5f);// new Vector2(X + center.X, Y + center.Y / 1.5f);
                    color.A = 0;
                    float pulsing = 10f + (float)Math.Sin(time * 20f);
                    float baseScalePulse = 1.03f;
                    float flameHeight = 5f;
                    float distortionAmount = 2f;


                    for (float f = 0f; f < MathHelper.TwoPi; f += 0.79f)
                    {
                        float angle = f + (time * 2f % MathHelper.TwoPi);

                        float distortion = (float)Math.Sin((position.Y + f * 100f + time * 20f) * 0.05f) * distortionAmount;

                        Vector2 offset = new Vector2(
                            (float)Math.Cos(angle) * pulsing * 0.4f + distortion,
                            -(float)Math.Abs((float)Math.Sin(angle)) * flameHeight
                        );

                        float scaleVariation = 0.95f + 0.05f * (float)Math.Sin(time * 15f + f * 2f);
                        Vector2 vscale = new Vector2(baseScalePulse * scaleVariation) * scale;

                        Color flameLayerColor = new Color(
                            (int)(color.R * 0.5f),
                            (int)(color.G * 0.5f),
                            (int)(color.B * 0.5f),
                            (int)(color.A * 0.5f)
                        );

                        ChatManager.DrawColorCodedString(
                            spriteBatch,
                            font,
                            text,
                            position + offset,
                            flameLayerColor,
                            0,
                            Vector2.Zero,
                            vscale
                        );
                    }

                    // Draw crisp center text
                    ChatManager.DrawColorCodedString(
                        spriteBatch,
                        font,
                        text,
                        position,
                        color,
                        0,
                        Vector2.Zero,
                        new Vector2(baseScalePulse)
                    );

                    color.A = 255;


                    var bloomColor = ColorTool.Rainbowing(time * 4 - 0.9f);

                    spriteBatch.Draw(crystalTextGlow, glowPosition, null, BloomClr, MathHelper.PiOver2, new Vector2(6f, 33f),
                       new Vector2(1.6f, fontSize.X / crystalTextGlow.Height * 1.2f), SpriteEffects.None, 0f);

                    ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, Color.Lerp(color,Color.White,0.67f), 0, Vector2.Zero, new(scale));
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

            if (Item.expert)
                textColor = Main.DiscoColor;

            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new(X, Y), textColor, 0, Vector2.Zero, baseScale, out _, -1, true);

            return;
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
            -2 => ModContent.RarityType<BurnishedAuric>(),
            -1 => ModContent.RarityType<HotPink>(),
            _ => Type,
        };*/
    }
}
