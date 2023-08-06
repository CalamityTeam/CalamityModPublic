using System;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class OldDukeScalesFatigue : CooldownHandler
    {
        public float CompletionPercentage => MathF.Round(100 - instance.Completion * 100);
        private bool IsPlayerTired => instance.player.GetModPlayer<OldDukeScalesPlayer>().IsTired;

        private float TextXOffset => CompletionPercentage > 99 ? -18 : CompletionPercentage > 9 ? -15 : -12;
        private Vector2 TextPosition => new(TextXOffset, 25);
        private Color TextColor => IsPlayerTired ? Color.Red : Color.White;
        private Color TextBorderColor = Color.Black;

        public static new string ID => "OldDukeScalesFatigue";
        public override bool CanTickDown => false;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override bool ShouldDisplay => instance.player.GetModPlayer<OldDukeScalesPlayer>().OldDukeScalesOn || IsPlayerTired;
        public override string Texture => "CalamityMod/Cooldowns/" + ID;

        public override Color CooldownStartColor => IsPlayerTired ? Color.Red : Color.Lerp(Color.Green, Color.Red, instance.Completion);
        public override Color CooldownEndColor => IsPlayerTired ? Color.Red : Color.Lerp(Color.Green, Color.Red, instance.Completion);

        public override void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.DrawExpanded(spriteBatch, position, opacity, scale);

            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, CompletionPercentage.ToString() + "%", position + TextPosition, TextColor, TextBorderColor);
        }

        public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.DrawCompact(spriteBatch, position, opacity, scale);

            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, CompletionPercentage.ToString() + "%", position + TextPosition, TextColor, TextBorderColor);
        }
    }
}
