using System;
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
    public class BurnishedAuric : ModRarity
    {
        // Burnished Auric is Rarity 15 (Once was known as Violet)
        public override Color RarityColor => TextClr * 2f;

        public static float MaxY = 4.5f;
        public static Color BloomClr = new Color(48, 33, 4, 0);
        public static Color TextClr = new Color(157, 110, 11, 255);
        static float lastFlashTime = 0f;
        static bool isFlashing = false;

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
                    var borderColor = color * 2f;
                    var coreColor = new Color(77, 0, 33);
                    var shineColor = new Color(254, 231, 117);
                    if (isFlashing)
                    {
                        shineColor = new Color(90, 207, 255);
                        position += Main.rand.NextVector2Circular(8f, 4.8f);
                    }

                    var pos = position;
                    using var lease = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice);
                    var matrix = spriteBatch.transformMatrix;
                    using (spriteBatch.Scope())
                    {
                        using (lease.Scope(clearColor: Color.Transparent))
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, text, pos, Color.White, 0, Vector2.Zero, new Vector2(scale));
                            spriteBatch.End();
                        }

                        //Draw the base text
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, matrix);
                        for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.TwoPi * 0.125f)
                        {
                            spriteBatch.Draw(lease.Target, Vector2.Zero + new Vector2(2, 0).RotatedBy(f), null, borderColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        spriteBatch.Draw(lease.Target, Vector2.Zero, null, coreColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.End();

                        //Draw the shine character-by-character
                        using (lease.Scope(clearColor: Color.Transparent))
                        {
                            string txt = "";
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                            foreach (var item in text)
                            {
                                pos = position;
                                pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;
                                float sin = (MathF.Sin(pos.X * 0.02f + Main.GlobalTimeWrappedHourly * -1.5f) + 1) * 0.5f;
                                var c = shineColor * MathF.Pow(sin, 120);
                                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, item.ToString(), pos, c, 0, Vector2.Zero, new Vector2(scale));
                                txt += item;
                            }
                            spriteBatch.End();
                        }

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, matrix);
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
            float flashChance = 0.005f; // Low chance per frame
            float flashDuration = 0.2f; // Flash lasts ~0.2 seconds

            if (Main.GameUpdateCount - lastFlashTime > flashDuration * 60)
            {
                if (Main.rand.NextFloat() < flashChance)
                {
                    isFlashing = true;
                    lastFlashTime = Main.GameUpdateCount;
                }
                else
                {
                    isFlashing = false;
                }
            }
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

            if (isFlashing)
            {
                textColor = new Color(0, 183, 241, 50);
                lightColor = new Color(14, 255, 255, 50);
            }

            textColor.A = 255;
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new(X, Y), textColor, 0, Vector2.Zero, baseScale, out _, -1, true);

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
            -2 => ModContent.RarityType<PureGreen>(),
            -1 => ModContent.RarityType<CosmicPurple>(),
            1 => ModContent.RarityType<HotPink>(),
            2 => ModContent.RarityType<CalamityRed>(),
            _ => Type,
        };*/
    }
}
