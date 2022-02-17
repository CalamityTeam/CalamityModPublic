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
    public class SandCloakCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Sand Cloak Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SandCloak";
        public override Color OutlineColor => new Color(209, 176, 114);
        public override Color CooldownColorStart => new Color(100, 64, 44);
        public override Color CooldownColorEnd => new Color(132, 95, 54);


        public SandCloakCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}