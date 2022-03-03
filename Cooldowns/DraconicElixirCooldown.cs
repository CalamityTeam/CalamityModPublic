using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class DraconicElixirCooldown : Cooldown
    {
        public override string SyncID => "DraconicElixir";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Draconic Surge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/DraconicElixir";
        public override Color OutlineColor => Color.Lerp(new Color(141, 199, 90), new Color(221, 187, 106), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownStartColor => new Color(165, 22, 46);
        public override Color CooldownEndColor => new Color(216, 103, 43);


        public DraconicElixirCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}