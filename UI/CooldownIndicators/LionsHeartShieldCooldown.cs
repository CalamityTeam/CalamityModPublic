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
    public class LionsHeartShieldCooldown : CooldownIndicator
    {
        public override string SyncID => "LionsHeartShield";
        public override bool DisplayMe => true;
        public override string Name => "Energy Shell Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/LionsHeartShield";
        public override Color OutlineColor => new Color(232, 239, 239);
        public override Color CooldownColorStart => new Color(17, 242, 244);
        public override Color CooldownColorEnd => Color.White;

        public LionsHeartShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}