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
    public class GodslayerDashCooldown : CooldownIndicator
    {
        public override string SyncID => "GodslayerDash";
        public override bool DisplayMe => true;
        public override string Name => "Godslayer Dash Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/GodslayerDash";
        public override Color OutlineColor => Color.Lerp(new Color(173, 66, 203), new Color(252, 109, 202), Completion);
        public override Color CooldownColorStart => new Color(252, 109, 202);
        public override Color CooldownColorEnd => new Color(119, 254, 254);


        public GodslayerDashCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}