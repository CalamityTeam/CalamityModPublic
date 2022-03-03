using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class CounterScarfCooldown : Cooldown
    {
        public override string SyncID => "CounterScarf";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Scarf Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/CounterScarf";
        public override Color OutlineColor => Color.Lerp(new Color(255, 115, 178), new Color(255, 76, 76), (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
        public override Color CooldownStartColor => new Color(194, 75, 97);
        public override Color CooldownEndColor => new Color(255, 76, 76);


        public CounterScarfCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}