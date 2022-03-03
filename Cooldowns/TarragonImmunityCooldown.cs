using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class TarragonImmunityCooldown : Cooldown
    {
        public override string SyncID => "TarraImmune";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Tarragon Immunity Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/TarragonImmunity";
        public override Color OutlineColor => new Color(215, 182, 82);
        public override Color CooldownStartColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - Completion);

        public TarragonImmunityCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}