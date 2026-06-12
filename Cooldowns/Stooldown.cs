using System;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Cooldowns
{
    public class Stooldown : CooldownHandler
    {
        public static new string ID => "Stooldown";
        public override bool ShouldDisplay => true;
        public override bool CanTickDown => true;
        public override void Tick()
        {
            base.Tick();
        }
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/Stooldown";
        public override Color OutlineColor => Color.SandyBrown;
        public override Color CooldownStartColor => Color.Brown;
        public override Color CooldownEndColor => Color.Khaki;

        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Item/StooldownResolve");
    }
}
