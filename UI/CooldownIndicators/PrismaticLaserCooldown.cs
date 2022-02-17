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
    public class PrismaticLaserCooldown : CooldownIndicator
    {
        public override string SyncID => "PrismaticLaser";
        public override bool DisplayMe => true;
        public override string Name => "Prismatic Laser Attack Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PrismaticLaser";
        public override Color OutlineColor => rainbowMode;
        public override Color CooldownColorStart => rainbowMode;
        public override Color CooldownColorEnd => rainbowMode;

        internal Color rainbowMode => MulticolorLerp(Main.GlobalTime % 1, new Color[] { new Color(103, 244, 251), new Color(255, 167, 236), new Color(255, 225, 136);

        public PrismaticLaserCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}