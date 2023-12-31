﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Cooldowns
{
    public class UniverseSplitter : CooldownHandler
    {
        public static new string ID => "UniverseSplitter";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText($"UI.Cooldowns.{ID}");
        public override string Texture => "CalamityMod/Cooldowns/UniverseSplitter";
        public override Color OutlineColor => rainbowMode;
        public override Color CooldownStartColor => rainbowMode;
        public override Color CooldownEndColor => rainbowMode;

        internal Color rainbowMode => MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.3f % 1, new Color[] { new Color(236, 202, 255), new Color(192, 245, 255), new Color(255, 194, 205) });
    }
}
