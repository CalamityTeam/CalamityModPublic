using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class SilvaReviveCooldown : Cooldown
    {
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Silva Revive Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SilvaRevive";
        public override Color OutlineColor => new Color(151, 211, 152);
        public override Color CooldownStartColor => new Color(226, 188, 74);
        public override Color CooldownEndColor => new Color(151, 211, 152);

        public override bool CanTickDown => !CalamityPlayer.areThereAnyDamnBosses && !CalamityPlayer.areThereAnyDamnEvents;

        public SilvaReviveCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}