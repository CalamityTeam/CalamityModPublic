using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class RogueBooster : CooldownHandler
    {
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Rogue Booster Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownStartColor => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - instance.Completion);

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public RogueBooster(CooldownInstance? c, string skin = "") : base(c)
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