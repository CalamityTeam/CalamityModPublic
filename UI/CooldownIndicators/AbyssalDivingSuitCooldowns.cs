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
    public class DivingPlatesBreaking : CooldownIndicator
    {
        public override bool DisplayMe => AfflictedPlayer.Calamity().abyssalDivingSuit;
        public override string Name => "Abyssal Diving Suit Plates Durability";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/AbyssalDivingSuitBreakingPlates";
        public override string TextureOutline => "CalamityMod/UI/CooldownIndicators/AbyssalDivingSuitBrokenPlatesOutline";
        public override string TextureOverlay => "CalamityMod/UI/CooldownIndicators/AbyssalDivingSuitBrokenPlatesOverlay";
        public override Color OutlineColor => TimeLeft == 0 ? new Color(147, 218, 183) : TimeLeft == 1 ? new Color(233, 190, 134) : new Color(220, 111, 94);
        public override Color CooldownColorStart => Color.Lerp(new Color(160, 174, 174), new Color(192, 11, 107), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(160, 174, 174), new Color(192, 11, 107), Completion);
        public override bool CanTickDown => !AfflictedPlayer.Calamity().abyssalDivingSuit;

        public DivingPlatesBreaking(int duration, Player player) : base(duration, player)
        {
        }

        public override bool UseCustomDrawCompact => true;

        public override void CustomDrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = GetTexture(Texture);
            Texture2D outline = GetTexture(TextureOutline);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            DrawBorderStringEightWay(spriteBatch, Main.fontMouseText, (3 - TimeLeft).ToString(), position + new Vector2(-3, -3) * scale, CooldownColorStart, Color.Black, scale);
        }
    }

    public class DivingPlatesBroken : CooldownIndicator
    {
        public override string SyncID => "AbyssalDivingBrokenPlates";
        public override bool DisplayMe => true;
        public override string Name => "Abyssal Diving Suit Broken Plates";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/AbyssalDivingSuitBrokenPlates";
        public override Color OutlineColor => new Color(194, 173, 146);
        public override Color CooldownColorStart => Color.Lerp(new Color(91, 121, 150), new Color(30, 50, 77), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(91, 121, 150), new Color(30, 50, 77), Completion);


        public DivingPlatesBroken(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCooldownEnd()
        {
            AfflictedPlayer.Calamity().abyssalDivingSuitPlateHits = 0;
        }
    }
}