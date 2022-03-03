using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class GodslayerDashCooldown : Cooldown
    {
        public override string SyncID => "GodslayerDash";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Godslayer Dash Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/GodslayerDash";
        public override Color OutlineColor => Color.Lerp(new Color(173, 66, 203), new Color(252, 109, 202), Completion);
        public override Color CooldownStartColor => new Color(252, 109, 202);
        public override Color CooldownEndColor => new Color(119, 254, 254);


        public GodslayerDashCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}