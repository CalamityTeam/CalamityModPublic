using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class TarragonCloakCooldown : Cooldown
    {
        public override string SyncID => "TarraCloak";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Tarragon Cloak Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/TarragonCloak";
        public override Color OutlineColor => new Color(158, 204, 173);
        public override Color CooldownStartColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - Completion);

        public TarragonCloakCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}