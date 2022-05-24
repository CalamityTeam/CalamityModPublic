using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class PlagueBlackout : CooldownHandler
    {
        public static new string ID => "PlagueBlackout";
        public override bool ShouldDisplay => instance.timeLeft <= 1500;
        public override string DisplayName => "Plague Blackout Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/PlagueBlackout";
        public override Color OutlineColor => new Color(174, 237, 122);
        public override Color CooldownStartColor => Color.DarkSlateGray;
        public override Color CooldownEndColor => Color.DarkSlateGray;
        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Custom/AbilitySounds/PlagueReaperRecharge");

        public override void OnCompleted()
        {
            Vector2 pos = instance.player.position;
            int w = instance.player.width;
            int h = instance.player.height;
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(pos, w, h, 89, 0, 0, 100, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 6.6f;
            }
        }


        //The cooldown is only displayed for the last 1500 frames (the other 300 are used for the actual blackout effect), so adjust the completion of the cooldown to start at 1500 frames and not earlier
        private float AdjustedCompletion => instance.timeLeft > 1500 ? 0 : 1 - instance.timeLeft / 1500f;

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
}
