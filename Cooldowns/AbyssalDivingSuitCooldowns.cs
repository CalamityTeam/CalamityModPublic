using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class DivingPlatesBreaking : Cooldown
    {
        private static Color phase0Color = new Color(147, 218, 183);
        private static Color phase1Color = new Color(233, 190, 134);
        private static Color phase2Color = new Color(220, 111, 94);
        private static Color ringColorLerpStart = new Color(160, 174, 174);
        private static Color ringColorLerpEnd = new Color(192, 11, 107);
        
        public override bool CanTickDown(CooldownInstance instance) => !instance.player.Calamity().abyssalDivingSuit;
        public override bool ShouldDisplay(CooldownInstance instance) => instance.player.Calamity().abyssalDivingSuit;
        public override string DisplayName(CooldownInstance instance) => "Abyssal Diving Suit Plates Durability";
        public override string Texture => "CalamityMod/Cooldowns/AbyssalDivingSuitBreakingPlates";
        public override string OutlineTexture => "CalamityMod/Cooldowns/AbyssalDivingSuitBrokenPlatesOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/AbyssalDivingSuitBrokenPlatesOverlay";
        public override Color GetOutlineColor(CooldownInstance instance)
        {
            if (instance.timeLeft == 0)
                return phase0Color;
            else if (instance.timeLeft == 1)
                return phase1Color;
            return phase2Color;
        }
        public override Color GetCooldownStartColor(CooldownInstance instance) => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color GetCooldownEndColor(CooldownInstance instance) => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);

        public override bool UseCustomCompactDraw => true;

        public override void DrawCompact(CooldownInstance instance, SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = GetTexture(Texture);
            Texture2D outline = GetTexture(OutlineTexture);
            Color outlineColor = GetOutlineColor(instance);
            Color startColor = GetCooldownStartColor(instance);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, outlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            DrawBorderStringEightWay(spriteBatch, Main.fontMouseText, (3 - instance.timeLeft).ToString(), position + new Vector2(-3, -3) * scale, startColor, Color.Black, scale);
        }
    }

    public class DivingPlatesBroken : Cooldown
    {
        private static Color outlineColor = new Color(194, 173, 146);
        private static Color ringColorLerpStart = new Color(91, 121, 150);
        private static Color ringColorLerpEnd = new Color(30, 50, 77);

        public override string ID => "AbyssalDivingBrokenPlates";
        public override bool ShouldDisplay(CooldownInstance instance) => true;
        public override string DisplayName(CooldownInstance instance) => "Abyssal Diving Suit Broken Plates";
        public override string Texture => "CalamityMod/Cooldowns/AbyssalDivingSuitBrokenPlates";
        public override Color GetOutlineColor(CooldownInstance instance) => outlineColor;
        public override Color GetCooldownStartColor(CooldownInstance instance) => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color GetCooldownEndColor(CooldownInstance instance) => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);

        public override void OnCompleted(CooldownInstance instance) => instance.player.Calamity().abyssalDivingSuitPlateHits = 0;
    }
}