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
    public class EvasionScarfCooldown : CooldownIndicator
    {
        public override string SyncID => "EvasionScarf";
        public override bool DisplayMe => true;
        public override string Name => "Scarf Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/EvasionScarf";
        public override Color OutlineColor => Color.Lerp(new Color(255, 194, 150), new Color(255, 160, 150), (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(132, 23, 32);
        public override Color CooldownColorEnd => new Color(164, 52, 45);

        public EvasionScarfCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}