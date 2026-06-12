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
    public sealed class SmallTextHandler : AbstractTagHandler<SmallTextHandler>
    {
        protected override string[] TagNames { get; } = ["scale"];
        public override TextSnippet Parse(string text, Color baseColor = new(), string options = null)
        {
            if (float.TryParse(options, out float result))
                return new SmallTextSnippet(text, baseColor,result);

            return new SmallTextSnippet(text, baseColor);
        }
    }
    public sealed class SmallTextSnippet(string text) : TextSnippet
    {
        public SmallTextSnippet( string text, Color color, float scale = 0.5f) : this(text)
        {
            textColor = color;
            Scale = scale;
        }

        Color textColor = Color.White;
        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
        {
            color = textColor;

            size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * Scale);

            if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0) && Color != Color.Transparent)
            {
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, position, color, 0, Vector2.Zero, new Vector2(scale));
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
