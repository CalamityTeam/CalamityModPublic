using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using CalamityMod.CalPlayer;

namespace CalamityMod.UI.CooldownIndicators
{
    public class UniverseSplitterCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Universe Splitter Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/UniverseSplitter";
        public override Color OutlineColor => rainbowMode;
        public override Color CooldownColorStart => rainbowMode;
        public override Color CooldownColorEnd => rainbowMode;

        internal Color rainbowMode => MulticolorLerp(Main.GlobalTime * 0.3f % 1, new Color[] { new Color(236, 202, 255), new Color(192, 245, 255), new Color(255, 194, 205) });

        public UniverseSplitterCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}