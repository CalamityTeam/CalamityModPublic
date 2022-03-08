using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Cooldowns
{
    public class BloodflareRangedSet : CooldownHandler
    {
        public static string ID => "BloodflareRangedSet";
        public BloodflareRangedSet(CooldownInstance? c) : base(c) { }

        public override bool ShouldDisplay => true;
        public override string DisplayName => "Bloodflare Soul Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/BloodflareSoul";
        public override Color OutlineColor => new Color(255, 205, 219);
        public override Color CooldownStartColor => new Color(216, 60, 90);
        public override Color CooldownEndColor => new Color(251, 106, 150);

        public override LegacySoundStyle EndSound => instance.player.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/BloodflareRangerRecharge");
    }
}