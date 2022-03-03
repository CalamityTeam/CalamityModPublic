using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class ProfanedSoulArtifactCooldown : Cooldown
    {
        public override string SyncID => "PSABurnout";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Soul Artifact Burn Out";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/ProfanedSoulArtifact";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownStartColor => new Color(181, 136, 177);
        public override Color CooldownEndColor => new Color(255, 194, 161);

        public ProfanedSoulArtifactCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}