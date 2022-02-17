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
using Terraria.ModLoader;

namespace CalamityMod.UI.CooldownIndicators
{
    public class BrimflameFrenzyCooldown : CooldownIndicator
    {
        public override string SyncID => "BrimflameFrenzy";
        public override bool DisplayMe => true;
        public override string Name => "Brimflame Frenzy Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BrimflameFrenzy";
        public override Color OutlineColor => new Color(211, 124, 93);
        public override Color CooldownColorStart => Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - Completion);

        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/BrimflameRecharge");

        public BrimflameFrenzyCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override bool UseCustomDraw => true;

        //Add red eyes to the icon
        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.CustomDraw(spriteBatch, position, opacity, scale);

            Texture2D sprite = GetTexture(Texture + "Eyes");
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.Crimson * opacity * (1 - Completion), 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}