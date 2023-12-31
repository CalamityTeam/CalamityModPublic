﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class BrimflameFrenzy : CooldownHandler
    {
        public static new string ID => "BrimflameFrenzy";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/BrimflameFrenzy";
        public override Color OutlineColor => new Color(211, 124, 93);
        public override Color CooldownStartColor => Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - instance.Completion);

        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Custom/AbilitySounds/BrimflameRecharge");


        //Add red eyes to the icon
        public override void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.DrawExpanded(spriteBatch, position, opacity, scale);

            Texture2D sprite = Request<Texture2D>(Texture + "Eyes").Value;
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.Crimson * opacity * (1 - instance.Completion), 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}
