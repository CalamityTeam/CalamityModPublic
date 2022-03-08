using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class SilvaRevive : CooldownHandler
    {
        public SilvaRevive(CooldownInstance? c) : base(c) { }

        public override bool ShouldDisplay => true;
        public override string DisplayName => "Silva Revive Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/SilvaRevive";
        public override Color OutlineColor => new Color(151, 211, 152);
        public override Color CooldownStartColor => new Color(226, 188, 74);
        public override Color CooldownEndColor => new Color(151, 211, 152);

        public override bool CanTickDown => !CalamityPlayer.areThereAnyDamnBosses && !CalamityPlayer.areThereAnyDamnEvents;
    }
}