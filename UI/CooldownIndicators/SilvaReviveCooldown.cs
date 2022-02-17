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
    public class SilvaReviveCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Silva Revive Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SilvaRevive";
        public override Color OutlineColor => new Color(151, 211, 152);
        public override Color CooldownColorStart => new Color(226, 188, 74);
        public override Color CooldownColorEnd => new Color(151, 211, 152);

        public override bool CanTickDown => !CalamityPlayer.areThereAnyDamnBosses && !CalamityPlayer.areThereAnyDamnEvents;

        public SilvaReviveCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}