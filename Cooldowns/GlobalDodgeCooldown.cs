using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class GlobalDodgeCooldown : Cooldown
    {
        public override string SyncID => "GlobalDodge";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Dodge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownStartColor => cooldownColorStart;
        public override Color CooldownEndColor => cooldownColorEnd;

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public GlobalDodgeCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "abyssmirror":
                    skinTexture = "AbyssEvade";
                    outlineColor = new Color(125, 157, 149);
                    cooldownColorStart = new Color(167, 147, 151);
                    cooldownColorEnd = new Color(217, 209, 195);
                    break;

                case "eclipsemirror":
                    skinTexture = "EclipseEvade";
                    outlineColor = new Color(152, 206, 248);
                    cooldownColorStart = new Color(255, 192, 71);
                    cooldownColorEnd = new Color(255, 255, 151);
                    break;

                default:
                    skinTexture = "GlobalDodge";
                    outlineColor = Color.White;
                    cooldownColorStart = Color.CornflowerBlue;
                    cooldownColorEnd = Color.White;
                    break;
            }
        }
    }
}