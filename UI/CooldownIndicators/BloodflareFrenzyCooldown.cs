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
using Terraria.ModLoader;

namespace CalamityMod.UI.CooldownIndicators
{
    public class BloodflareFrenzyCooldown : CooldownIndicator
    {
        public override string SyncID => "BloodflareFrenzy";
        public override bool DisplayMe => true;
        public override string Name => "Blood Frenzy Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BloodflareFrenzy";
        public override Color OutlineColor => new Color(229, 171, 124);
        public override Color CooldownColorStart => Color.Lerp(new Color(149, 127, 109), new Color(220, 101, 101), 1 - Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(149, 127, 109), new Color(220, 101, 101), 1 - Completion);

        public BloodflareFrenzyCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}