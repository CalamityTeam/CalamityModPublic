using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class FleshTotem : CooldownHandler
    {
        public static new string ID => "FleshTotem";

        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownStartColor => cooldownColorStart;
        public override Color CooldownEndColor => cooldownColorEnd;

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public FleshTotem() : this("") { }
        public FleshTotem(string skin)
        {
            switch (skin)
            {
                // TODO -- UNUSED, Core of the Blood God no longer exists.
                case "bloodgod":
                    skinTexture = "BloodGodTotem";
                    outlineColor = new Color(255, 162, 205);
                    cooldownColorStart = new Color(193, 205, 255);
                    cooldownColorEnd = new Color(255, 193, 219);
                    break;

                default:
                    skinTexture = "FleshTotem";
                    outlineColor = new Color(157, 248, 234);
                    cooldownColorStart = new Color(111, 169, 241);
                    cooldownColorEnd = new Color(111, 169, 241);
                    break;
            }
        }
    }
}
