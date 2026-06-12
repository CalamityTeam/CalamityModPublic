using System;
using System.Collections.Generic;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
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
    public class ExoticRainbow : ModRarity
    {
        // Exotic Rainbow is Rarity 17 (same as Calamity Red)
        public override Color RarityColor => TextClr * 2f;

        public static float MaxY = 4.5f;
        public static Color BloomClr = new Color(180, 20, 75, 0);
        public static Color TextClr = new Color(242, 27, 27, 255);

        public sealed class CustomTextSnippet(string text) : TextSnippet
        {
            public bool IsExpert = false;
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

                    List<Color> eColors = new List<Color>()
                    {
                        new Color(255,107,107), //Ares
                        new Color(125,196,225), //Thanatos
                        new Color(211,235,108), //Apollo
                        //new Color(255,160,71), //Artemis
                    };

                    if (IsExpert)
                        eColors = new List<Color>()
                        {
                        new Color(255,70,70),
                        new Color(255,70,255),
                        new Color(70,70,255),
                        new Color(70,255,255),
                        new Color(70,255,90),
                        new Color(255,255,70)
                    };


                    var pos = position;
                    using var lease = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice);
                    var matrix = spriteBatch.transformMatrix;
                    using (spriteBatch.Scope())
                    {
                        using (lease.Scope(clearColor: Color.Transparent))
                        {
                            string txt = "";
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                            foreach (var item in text)
                            {

                                pos = position;
                                pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;

                                float rate = Main.GlobalTimeWrappedHourly * (IsExpert ? 2 : 1) + pos.X * (IsExpert ? 0.01f : 0.005f);
                                int colorIndex = (int)(rate / 2 % eColors.Count);
                                Color currentColor = eColors[colorIndex];
                                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                                Color usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : MathF.Round(rate % 1f));

                                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, item.ToString(), pos, usedColor, 0, Vector2.Zero, new Vector2(scale));
                                txt += item;
                            }
                            spriteBatch.End();
                        }

                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, matrix);
                        float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2 / MathHelper.Pi);
                        sine = (float)Math.Pow(MathHelper.Lerp(sine, 0, 0.35f), 5);
                        int draws = 16;
                        for (int i = 0; i < draws; i++)
                        {
                            Vector2 backPosition = (MathHelper.TwoPi * i / (float)draws + Main.GlobalTimeWrappedHourly * 1.7f).ToRotationVector2() * (4 + 16 * sine);
                            spriteBatch.Draw(lease.Target, Vector2.Zero + backPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }

                        Main.spriteBatch.End();

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, matrix);
                        for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.TwoPi * 0.125f)
                        {
                            spriteBatch.Draw(lease.Target, Vector2.Zero + new Vector2(2, 0).RotatedBy(f), null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.End();
                    }
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
                    snippets[i] = new CustomTextSnippet(textSnippet.Text) { IsExpert = Item.expert };
                }
            }


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
