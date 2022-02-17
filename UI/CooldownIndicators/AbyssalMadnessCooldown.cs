using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using CalamityMod.CalPlayer;

namespace CalamityMod.UI.CooldownIndicators
{
    public class OmegaBlueCooldown : CooldownIndicator
    {
        public override string SyncID => "OmegaBlue";
        public override bool DisplayMe => true;
        public override string Name => TimeLeft > 1500 ? "Abyssal Madness" : "Abyssal Madness Cooldown";
        public override string Texture => TimeLeft > 1500 ? "CalamityMod/UI/CooldownIndicators/AbyssalMadnessActive" : "CalamityMod/UI/CooldownIndicators/AbyssalMadness";
        public override string TextureOutline => "CalamityMod/UI/CooldownIndicators/AbyssalMadnessOutline";
        public override string TextureOverlay => "CalamityMod/UI/CooldownIndicators/AbyssalMadnessOverlay";
        public override Color OutlineColor => TimeLeft > 1500 ? new Color(231, 164, 1) : new Color(72, 135, 205);
        public override Color CooldownColorStart => TimeLeft > 1500 ? Color.Lerp(new Color(98, 110, 179), new Color(216, 176, 80), (TimeLeft - 1500) / 300f) : new Color(98, 110, 179);
        public override Color CooldownColorEnd => TimeLeft > 1500 ? Color.Lerp(new Color(179, 132, 98), new Color(216, 176, 80), (TimeLeft - 1500) / 300f) : new Color(179, 132, 98);
        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/OmegaBlueRecharge");

        public OmegaBlueCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCooldownEnd()
        {
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(AfflictedPlayer.position, AfflictedPlayer.width, AfflictedPlayer.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 6.6f;
            }
        }

        //Charge down at first, and then charge back up
        private float AdjustedCompletion => TimeLeft > 1500 ? (TimeLeft - 1500) / 300f : 1 - TimeLeft / 1500f;

        public override void ApplyBarShaders(float opacity)
        {
            //Use the adjusted completion
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(AdjustedCompletion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownColorStart);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownColorEnd);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
        }

        public override bool UseCustomDrawCompact => true;

        public override void CustomDrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = GetTexture(Texture);
            Texture2D outline = GetTexture(TextureOutline);
            Texture2D overlay = GetTexture(TextureOverlay);

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