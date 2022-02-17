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
    public class PermafrostConcoctionCooldown : CooldownIndicator
    {
        public override string SyncID => "ConcoctionCooldown";
        public override bool DisplayMe => true;
        public override string Name => "Permafrost's Concoction Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PermafrostConcoction";
        public override Color OutlineColor => new Color(0, 218, 255);
        public override Color CooldownColorStart => new Color(144, 184, 205);
        public override Color CooldownColorEnd => new Color(232, 246, 254);

        public PermafrostConcoctionCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}