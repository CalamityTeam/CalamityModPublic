using System;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Rarities;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace CalamityMod.ChatTags
{
    // This is a tag handler for storing custom color effects for text all within the "ceffect" tag; the parameter determining the actual effect in use.
    // i.e [ceffect/darksun:Hello World] would apply darksun effect to "Hello World".
    public sealed class CustomColorEffectHandler : AbstractTagHandler<CustomColorEffectHandler>
    {
        protected override string[] TagNames { get; } = ["ceffect", "ce"];
        public override TextSnippet Parse(string text, Color baseColor = new(), string options = null)
        {
            if (!CalamityClientConfig.Instance.TextEffects)
                return new TextSnippet(text);
            if (options.Equals("darksun", StringComparison.OrdinalIgnoreCase))
                return new DarksunTextSnippet(text);
            if (options.Equals("drunk", StringComparison.OrdinalIgnoreCase))
                return new DrunkTextSnippet(text);
            if (options.Equals("dog", StringComparison.OrdinalIgnoreCase))
                return new DoGTextSnippet(text);
            if (options.Equals("cosmic", StringComparison.OrdinalIgnoreCase))
                return new CosmicPurple.CustomTextSnippet(text);
            if (options.Equals("auric", StringComparison.OrdinalIgnoreCase))
                return new BurnishedAuric.CustomTextSnippet(text);
            if (options.Equals("calamity", StringComparison.OrdinalIgnoreCase))
                return new CalamityRed.CustomTextSnippet(text);
            if (options.Equals("exo", StringComparison.OrdinalIgnoreCase))
                return new ExoticRainbow.CustomTextSnippet(text);
            return new TextSnippet(text);
        }
    }

    public sealed class DarksunTextSnippet(string text) : TextSnippet
    {
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                var font = FontAssets.MouseText.Value;
                var BorderColor = new Color(255, 191, 73);
                var HazeColor = new Color(238, 226, 153);

                for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.TwoPi * 0.05f)
                {
                    ChatManager.DrawColorCodedString(spriteBatch, font, text, position + new Vector2(2, 0).RotatedBy(f + Main.GlobalTimeWrappedHourly), Color.Lerp(BorderColor, HazeColor, (Main.mouseTextColor - 190) / 65f * 0.1f), 0f, Vector2.Zero, new Vector2(scale));
                }
                ChatManager.DrawColorCodedString(spriteBatch, font, text, position, Color.Black, 0f, Vector2.Zero, new Vector2(scale));
            }
            return true;
        }
        public override float GetStringLength(DynamicSpriteFont font)
        {
            float size = font.MeasureString(text).X;
            return size * Scale;
        }
    }

    public sealed class DrunkTextSnippet(string text) : TextSnippet
    {
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                var matrix = spriteBatch.transformMatrix;
                Main.spriteBatch.End(out var ss);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, matrix);

                for (int i = 0; i < 3; i++) // Draw 3 lines of differing opacity, color, and displacement.
                {
                    float timer = (float)Math.Sin(Main.GlobalTimeWrappedHourly + 1f) / 2f;

                    float angle = (Main.GlobalTimeWrappedHourly * 2f) + (i * MathHelper.TwoPi / 3f);
                    float radius = timer * (3f + i * 6f);
                    Vector2 offset = new Vector2((float)Math.Cos(angle) * 1.7f, (float)Math.Sin(angle) * 1f) * radius; // Offset in an elliptical pattern
                    Vector2 pos = position + offset;

                    color = CalamityUtils.MulticolorLerp(Math.Abs(timer + (i * 0.2f)) % 1f, Color.LightBlue, Color.LightGreen, (i == 3) ? Color.Blue : (i == 2) ? Color.Red : Color.White);
                    float opacity = 1f - (i * 0.2f);
                    float rotation = +(timer * (Main.GlobalTimeWrappedHourly * 0.00001f)); // This precision is intended

                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, pos, color * opacity, rotation, Vector2.Zero, new Vector2(scale));
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(ss);
            }
            return true;
        }
        public override float GetStringLength(DynamicSpriteFont font)
        {
            float size = font.MeasureString(text).X;
            return size * Scale;
        }
    }

    public sealed class DoGTextSnippet(string text) : TextSnippet
    {
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
            {
                var pos = position;
                using var lease = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice);
                string txt = "";
                var matrix = spriteBatch.transformMatrix;
                using (spriteBatch.Scope())
                {
                    using (lease.Scope(clearColor: Color.Transparent))
                    {

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, matrix);
                        foreach (var item in text)
                        {
                            pos = position;
                            pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;
                            float sin = MathHelper.SmoothStep(0, 1, (MathF.Sin(pos.X * 0.02f + Main.GlobalTimeWrappedHourly * -1.5f) + 1) * 0.5f);
                            var c = Color.Lerp(Color.Cyan, Color.Fuchsia, sin);
                            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, item.ToString(), pos + new Vector2(0, -2 + sin * 4), c, 0, Vector2.Zero, new Vector2(scale));
                            txt += item;
                        }
                        spriteBatch.End();
                    }

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                    foreach (var item in ChatManager.ShadowDirections)
                    {
                        spriteBatch.Draw(lease.Target, Vector2.Zero + Vector2.TransformNormal(item * 2,matrix), null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
}
