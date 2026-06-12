using CalamityMod.Items.Tools;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Cooldowns
{
    public class RelicOfResilienceCooldown : CooldownHandler
    {
        public static new string ID => "RelicOfResilienceCooldown";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/RelicOfResilienceCooldown";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(122, 66, 59);
        public override Color CooldownEndColor => new Color(165, 103, 87);

        public override void Tick()
        {
            if (instance.player.HeldItem.type == ModContent.ItemType<RelicOfResilience>())
                instance.timeLeft--;
        }
    }
}
