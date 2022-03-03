using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class SandCloakCooldown : Cooldown
    {
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Sand Cloak Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SandCloak";
        public override Color OutlineColor => new Color(209, 176, 114);
        public override Color CooldownStartColor => new Color(100, 64, 44);
        public override Color CooldownEndColor => new Color(132, 95, 54);


        public SandCloakCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}