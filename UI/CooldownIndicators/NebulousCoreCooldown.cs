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
    public class NebulousCoreCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Nebulous Core Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/NebulousCore";
        public override Color OutlineColor => Color.Lerp(new Color(252, 109, 203), new Color(58, 91, 146), Completion);
        public override Color CooldownColorStart => new Color(148, 62, 216);
        public override Color CooldownColorEnd => new Color(255, 187, 207);

        public NebulousCoreCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}