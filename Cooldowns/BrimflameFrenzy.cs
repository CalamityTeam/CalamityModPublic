using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class BrimflameFrenzy : CooldownHandler
    {
        public static string ID => "BrimflameFrenzy";
        public BrimflameFrenzy(CooldownInstance? c) : base(c) { }

        public override bool ShouldDisplay => true;
        public override string DisplayName => "Brimflame Frenzy Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/BrimflameFrenzy";
        public override Color OutlineColor => new Color(211, 124, 93);
        public override Color CooldownStartColor => Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - instance.Completion);

        public override LegacySoundStyle EndSound => instance.player.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/BrimflameRecharge");

        public override bool UseCustomExpandedDraw => true;

        //Add red eyes to the icon
        public override void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            base.DrawExpanded(spriteBatch, position, opacity, scale);

            Texture2D sprite = GetTexture(Texture + "Eyes");
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.Crimson * opacity * (1 - instance.Completion), 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}