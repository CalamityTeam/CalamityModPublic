using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Cooldowns
{
    public class AscendEffect : CooldownHandler
    {
        public static new string ID => "AscendEffect";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/AscendEffect";
        public override Color OutlineColor => new Color(50, 50, 50);
        public override Color CooldownStartColor => new Color(228, 97, 7);
        public override Color CooldownEndColor => new Color(248, 233, 94);

        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Item/AscendantOff");
    }
}
