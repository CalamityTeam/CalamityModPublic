using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.Cooldowns
{
    // TODO -- Why do the two scarves not use the same cooldown? They're both called "Scarf Cooldown".
    public class EvasionScarf : CooldownHandler
    {
        public static string ID => "EvasionScarf";
        public EvasionScarf(CooldownInstance? c) : base(c) { }

        public override bool ShouldDisplay => true;
        public override string DisplayName => "Scarf Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/EvasionScarf";
        public override Color OutlineColor => Color.Lerp(new Color(255, 194, 150), new Color(255, 160, 150), (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
        public override Color CooldownStartColor => new Color(132, 23, 32);
        public override Color CooldownEndColor => new Color(164, 52, 45);
    }
}