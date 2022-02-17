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
    public class RelicOfResilienceCooldown : CooldownIndicator
    {
        public override string SyncID => "RelicOfResilience";
        public override bool DisplayMe => true;
        public override string Name => "Relic of Resilience Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/RelicOfResilience";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownColorStart => new Color(122, 66, 59);
        public override Color CooldownColorEnd => new Color(165, 103, 87);

        public RelicOfResilienceCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}