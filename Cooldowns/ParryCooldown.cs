using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Cooldowns
{
    public class ParryCooldown : CooldownHandler
    {
        public static new string ID => "ParryCooldown";
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
        
        public ParryCooldown() : this("") { }
        public ParryCooldown(string skin)
        {
            switch (skin)
            {
                case "blazingcore":
                    skinTexture = "BlazingCoreParry";
                    outlineColor = new Color(255, 191, 73);
                    cooldownColorStart = new Color(181, 136, 177);
                    cooldownColorEnd = new Color(255, 194, 161);
                    break;
                
                //readd if spritework for the cooldown is done at a later point
                /*case "flamelickedshell":
                    skinTexture = "FlameLickedShellParry";
                    outlineColor = new Color(211, 124, 93);
                    cooldownColorStart = Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - (instance?.Completion ?? 0));
                    cooldownColorEnd = Color.Lerp(new Color(107, 6, 6), new Color(228, 78, 78), 1 - (instance?.Completion ?? 0));
                    break;*/
                default:
                    skinTexture = "ParryCooldown";
                    outlineColor = Color.White;
                    cooldownColorStart = Color.CornflowerBlue;
                    cooldownColorEnd = Color.White;
                    break;
            }
        }
    }
}
