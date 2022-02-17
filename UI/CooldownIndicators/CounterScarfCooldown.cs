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
    public class CounterScarfCooldown : CooldownIndicator
    {
        public override string SyncID => "CounterScarf";
        public override bool DisplayMe => true;
        public override string Name => "Scarf Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/CounterScarf";
        public override Color OutlineColor => Color.Lerp(new Color(255, 115, 178), new Color(255, 76, 76), (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(194, 75, 97);
        public override Color CooldownColorEnd => new Color(255, 76, 76);


        public CounterScarfCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}