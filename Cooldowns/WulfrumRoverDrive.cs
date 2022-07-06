using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Items.Accessories;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using System;

namespace CalamityMod.Cooldowns
{
    public class WulfrumRoverDriveDurability : CooldownHandler
    {
        private static Color ringColorLerpStart = new Color(49, 220, 221);
        private static Color ringColorLerpEnd = new Color(99, 226, 142);

        private float AdjustedCompletion => (instance.timeLeft) / (float)RoverDrive.ProtectionMatrixDurabilityMax;

        public static new string ID => "WulfrumRoverDriveDurability";
        public override bool CanTickDown => !instance.player.GetModPlayer<RoverDrivePlayer>().RoverDriveOn || instance.timeLeft <= 0;
        public override bool ShouldDisplay => instance.player.GetModPlayer<RoverDrivePlayer>().RoverDriveOn;
        public override string DisplayName => "Protective Matrix Durability";
        public override string Texture => "CalamityMod/Cooldowns/WulfrumRoverDriveActive";
        public override string OutlineTexture => "CalamityMod/Cooldowns/WulfrumRoverDriveOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/WulfrumRoverDriveOverlay";
        public override Color OutlineColor => new Color(112, 244, 244);
        public override Color CooldownStartColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override bool SavedWithPlayer => false;
        public override bool PersistsThroughDeath => false;


        public override void ApplyBarShaders(float opacity)
        {
            //Use the adjusted completion
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(AdjustedCompletion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownStartColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownEndColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
        }

        public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = Request<Texture2D>(Texture).Value;
            Texture2D outline = Request<Texture2D>(OutlineTexture).Value;
            Texture2D overlay = Request<Texture2D>(OverlayTexture).Value;

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * AdjustedCompletion);
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }

    public class WulfrumRoverDriveRecharge : CooldownHandler
    {
        private static Color ringColorLerpStart = new Color(194, 255, 57);
        private static Color ringColorLerpEnd = new Color(92, 187, 99);

        public static new string ID => "WulfrumRoverDriveRecharge";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Protective Matrix Recharge";
        public override string Texture => "CalamityMod/Cooldowns/WulfrumRoverDrive";
        public override bool SavedWithPlayer => false;
        public override bool PersistsThroughDeath => false;
        public override Color OutlineColor => new Color(194, 255, 67);
        public override Color CooldownStartColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);

        public override void OnCompleted() => instance.player.GetModPlayer<RoverDrivePlayer>().ProtectionMatrixDurability = RoverDrive.ProtectionMatrixDurabilityMax;
        public override SoundStyle? EndSound => null;
    }
}
