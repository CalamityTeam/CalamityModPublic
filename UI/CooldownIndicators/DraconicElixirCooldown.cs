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
    public class DraconicElixirCooldown : CooldownIndicator
    {
        public override string SyncID => "DraconicElixir";
        public override bool DisplayMe => true;
        public override string Name => "Draconic Surge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/DraconicElixir";
        public override Color OutlineColor => Color.Lerp(new Color(141, 199, 90), new Color(221, 187, 106), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(165, 22, 46);
        public override Color CooldownColorEnd => new Color(216, 103, 43);


        public DraconicElixirCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}