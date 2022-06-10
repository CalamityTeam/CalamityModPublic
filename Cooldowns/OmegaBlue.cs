using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class OmegaBlue : CooldownHandler
    {
        public static new string ID => "OmegaBlue";
        public override bool ShouldDisplay => true;
        public override string DisplayName => instance.timeLeft > 1500 ? "Abyssal Madness" : "Abyssal Madness Cooldown";
        public override string Texture => instance.timeLeft > 1500 ? "CalamityMod/Cooldowns/OmegaBlueActive" : "CalamityMod/Cooldowns/OmegaBlue";
        public override string OutlineTexture => "CalamityMod/Cooldowns/OmegaBlueOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/OmegaBlueOverlay";
        public override Color OutlineColor => instance.timeLeft > 1500 ? new Color(231, 164, 1) : new Color(72, 135, 205);
        public override Color CooldownStartColor => instance.timeLeft > 1500 ? Color.Lerp(new Color(98, 110, 179), new Color(216, 176, 80), (instance.timeLeft - 1500) / 300f) : new Color(98, 110, 179);
        public override Color CooldownEndColor => instance.timeLeft > 1500 ? Color.Lerp(new Color(179, 132, 98), new Color(216, 176, 80), (instance.timeLeft - 1500) / 300f) : new Color(179, 132, 98);
        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Custom/AbilitySounds/OmegaBlueRecharge");

        public override void OnCompleted()
        {
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(instance.player.position, instance.player.width, instance.player.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 6.6f;
            }
        }

        //Charge down at first, and then charge back up
        private float AdjustedCompletion => instance.timeLeft > 1500 ? (instance.timeLeft - 1500) / 300f : 1 - instance.timeLeft / 1500f;

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
