using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class RaiderBoost : CooldownHandler
    {
        public static new string ID => "RaidersBoost";

        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");

        public override string Texture => "CalamityMod/Cooldowns/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownStartColor => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(cooldownColorStart, cooldownColorEnd, 1 - instance.Completion);

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public RaiderBoost() : this("") { }
        public RaiderBoost(string skin)
        {
            switch (skin)
            {
                case "Nanotech":
                    skinTexture = "NanotechBoost";
                    outlineColor = new Color(14, 103, 129);
                    cooldownColorStart = new Color(85, 252, 229);
                    cooldownColorEnd = new Color(52,255,171);
                    break;

                case "Vampiric":
                    skinTexture = "VampiricTalismanBoost";
                    outlineColor = new Color(143, 27, 27);
                    cooldownColorStart = new Color(133, 5, 5);
                    cooldownColorEnd = new Color(255, 0, 0);
                    break;

                default:
                    skinTexture = "RaiderBoost";
                    outlineColor = new Color(122, 97, 77);
                    cooldownColorStart = new Color(168, 122, 86);
                    cooldownColorEnd = new Color(74, 60, 49);
                    break;
            }
        }
    }
}
