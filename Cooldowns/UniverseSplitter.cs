using Microsoft.Xna.Framework;
using Terraria;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Cooldowns
{
    public class UniverseSplitter : CooldownHandler
    {
        public UniverseSplitter(CooldownInstance? c) : base(c) { }

        public override bool ShouldDisplay => true;
        public override string DisplayName => "Universe Splitter Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/UniverseSplitter";
        public override Color OutlineColor => rainbowMode;
        public override Color CooldownStartColor => rainbowMode;
        public override Color CooldownEndColor => rainbowMode;

        internal Color rainbowMode => MulticolorLerp(Main.GlobalTime * 0.3f % 1, new Color[] { new Color(236, 202, 255), new Color(192, 245, 255), new Color(255, 194, 205) });
    }
}