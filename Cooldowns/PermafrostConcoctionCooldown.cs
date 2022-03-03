using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class PermafrostConcoctionCooldown : Cooldown
    {
        public override string SyncID => "ConcoctionCooldown";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Permafrost's Concoction Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PermafrostConcoction";
        public override Color OutlineColor => new Color(0, 218, 255);
        public override Color CooldownStartColor => new Color(144, 184, 205);
        public override Color CooldownEndColor => new Color(232, 246, 254);

        public PermafrostConcoctionCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}