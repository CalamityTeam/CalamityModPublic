using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace CalamityMod.Cooldowns
{
    public class BloodflareSoulCooldown : Cooldown
    {
        public override string SyncID => "BloodflareSoul";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Bloodflare Soul Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BloodflareSoul";
        public override Color OutlineColor => new Color(255, 205, 219);
        public override Color CooldownStartColor => new Color(216, 60, 90);
        public override Color CooldownEndColor => new Color(251, 106, 150);

        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/BloodflareRangerRecharge");

        public BloodflareSoulCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}