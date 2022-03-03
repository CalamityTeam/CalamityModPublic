using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class JetpackCooldown : Cooldown
    {
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Jet Boost Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownStartColor => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - Completion);
        public override Color CooldownEndColor => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - Completion);

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public JetpackCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "birb":
                    skinTexture = "BlunderBooster";
                    outlineColor = new Color(210, 180, 100);
                    cooldownColorStart = new Color(90, 67, 76);
                    cooldownColorEnd = new Color(255, 83, 145);
                    break;

                default:
                    skinTexture = "PlaguedFuelPack";
                    outlineColor = new Color(130, 190, 64);
                    cooldownColorStart = new Color(230, 64, 64);
                    cooldownColorEnd = new Color(209, 248, 62);
                    break;
            }
        }
    }
}