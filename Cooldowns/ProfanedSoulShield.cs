using System;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class ProfanedSoulShield : CooldownHandler
    {
        
        private CalamityPlayer CalPlayer => instance.player.Calamity();
        private int CorrectMaxDurability => CalPlayer.profanedCrystalBuffs
            ? ProfanedSoulCrystal.ShieldDurabilityMax
            : ProfanedSoulArtifact.ShieldDurabilityMax;

        //Slightly more readable and actually works which is nice
        private Color GetColor(bool start)
        {
            Color result;
            if (start)
            {
                result = CalPlayer.profanedCrystalBuffs ? new Color(247, 172, 131) : new Color(235, 178, 96);
            }
            else
            {
                result = CalPlayer.profanedCrystalBuffs ? new Color(255, 194, 161) : new Color(219, 179, 121);
            }
            return result;
        }
        
        private float AdjustedCompletion => instance.timeLeft / (float)CorrectMaxDurability;

        public static new string ID => "ProfanedSoulShieldDurability";
        public override bool CanTickDown => !CalPlayer.pSoulArtifact || instance.timeLeft <= 0;
        public override bool ShouldDisplay => (CalPlayer.pSoulArtifact && !CalPlayer.profanedCrystal) || CalPlayer.profanedCrystalBuffs;
        public override LocalizedText DisplayName => GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/ProfanedSoulShieldActive";
        public override string OutlineTexture => "CalamityMod/Cooldowns/ProfanedSoulShieldOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/ProfanedSoulShieldOverlay";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => Color.Lerp(GetColor(true), GetColor(false), instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(GetColor(true), GetColor(false), instance.Completion);
        public override bool SavedWithPlayer => false;
        public override bool PersistsThroughDeath => false;

        public override void ApplyBarShaders(float opacity)
        {
            // Use the adjusted completion
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(AdjustedCompletion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownStartColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownEndColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
        }

        public override void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.DrawExpanded(spriteBatch, position, opacity, scale);

            float Xoffset = instance.timeLeft > 9 ? -10f : -5;
            DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, instance.timeLeft.ToString(), position + new Vector2(Xoffset, 4) * scale, Color.Lerp(GetColor(true), Color.OrangeRed, 1 - instance.Completion), Color.Black, scale);
        }

        public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = Request<Texture2D>(Texture).Value;
            Texture2D outline = Request<Texture2D>(OutlineTexture).Value;
            Texture2D overlay = Request<Texture2D>(OverlayTexture).Value;

            // Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            // Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            // Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * AdjustedCompletion);
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            float Xoffset = instance.timeLeft > 9 ? -10f : -5;
            DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, instance.timeLeft.ToString(), position + new Vector2(Xoffset, 4) * scale, Color.Lerp(GetColor(true), Color.OrangeRed, 1 - instance.Completion), Color.Black, scale);
        }
    }

    public class ProfanedSoulShieldRecharge : CooldownHandler
    {
        private static Color ringColorLerpStart = new Color(217, 159, 78);
        private static Color ringColorLerpEnd = new Color(214, 185, 144);

        public static new string ID => "ProfanedSoulShieldRecharge";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/ProfanedSoulShieldRecharge";
        public override string OutlineTexture => "CalamityMod/Cooldowns/ProfanedSoulShieldOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/ProfanedSoulShieldOverlay";
        public override bool SavedWithPlayer => false;
        public override bool PersistsThroughDeath => false;
        public override Color OutlineColor => new Color(57, 195, 237);
        public override Color CooldownStartColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(ringColorLerpStart, ringColorLerpEnd, instance.Completion);
        
        public override void Tick() => instance.player.Calamity().playedProfanedSoulShieldSound = false;

        public override SoundStyle? EndSound => Providence.BurnStartSound;

        public override bool ShouldPlayEndSound => instance.player.Calamity().pSoulArtifact;

        // When the recharge period completes, grant 1 point of shielding immediately so the rest my refill normally.
        // The shield durability cooldown is added elsewhere, in Misc Effects.
        public override void OnCompleted()
        {
            CalamityPlayer modPlayer = instance.player.Calamity();
            if (modPlayer.pSoulShieldDurability <= 0)
                modPlayer.pSoulShieldDurability = 1;
        }
    }
}
