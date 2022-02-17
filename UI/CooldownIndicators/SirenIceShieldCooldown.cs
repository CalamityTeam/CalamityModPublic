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
    public class SirenIceShieldCooldown : CooldownIndicator
    {
        public override string SyncID => "SirenHeartShield";
        public override bool DisplayMe => true;
        public override string Name => "Ice Shield Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SirenIceShield";
        public override Color OutlineColor => Color.Lerp(new Color(163, 186, 198), new Color(146, 187, 255), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(124, 195, 214);
        public override Color CooldownColorEnd => new Color(147, 230, 253);


        public SirenIceShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}