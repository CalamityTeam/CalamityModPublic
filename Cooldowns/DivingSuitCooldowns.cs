using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class DivingPlatesBreaking : CooldownHandler
    {
        private static Color phase0Color = new Color(147, 218, 183);
        private static Color phase1Color = new Color(233, 190, 134);
        private static Color phase2Color = new Color(220, 111, 94);
        private static Color ringColorLerpStart = new Color(160, 174, 174);
        private static Color ringColorLerpEnd = new Color(192, 11, 107);

        public static new string ID => "DivingPlatesBreaking";
        public override bool CanTickDown => !instance.player.Calamity().abyssalDivingSuit;
        public override bool ShouldDisplay => instance.player.Calamity().abyssalDivingSuit;
        public override string DisplayName => "Abyssal Diving Suit Plates Durability";
        public override string Texture => "CalamityMod/Cooldowns/DivingSuitBreakingPlates";
        public override string OutlineTexture => "CalamityMod/Cooldowns/DivingSuitBrokenPlatesOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/DivingSuitBrokenPlatesOverlay";
        public override Color OutlineColor
        {
            get
            {
                if (instance.timeLeft == 0)
                    return phase0Color;
                else if (instance.timeLeft == 1)
                    return phase1Color;
                return phase2Color;
            }
        }
        public override Color CooldownStartColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);

        public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = GetTexture(Texture);
            Texture2D outline = GetTexture(OutlineTexture);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            DrawBorderStringEightWay(spriteBatch, Main.fontMouseText, (3 - instance.timeLeft).ToString(), position + new Vector2(-3, -3) * scale, CooldownStartColor, Color.Black, scale);
        }
    }

    public class DivingPlatesBroken : CooldownHandler
    {
        private static Color ringColorLerpStart = new Color(91, 121, 150);
        private static Color ringColorLerpEnd = new Color(30, 50, 77);

        public static new string ID => "DivingPlatesBroken";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Abyssal Diving Suit Broken Plates";
        public override string Texture => "CalamityMod/Cooldowns/DivingSuitBrokenPlates";
        public override Color OutlineColor => new Color(194, 173, 146);
        public override Color CooldownStartColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);

        public override void OnCompleted() => instance.player.Calamity().abyssalDivingSuitPlateHits = 0;
    }
}