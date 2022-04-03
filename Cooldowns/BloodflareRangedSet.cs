using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class BloodflareRangedSet : CooldownHandler
    {
        public static new string ID => "BloodflareRangedSet";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Bloodflare Soul Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/BloodflareRangedSet";
        public override Color OutlineColor => new Color(255, 205, 219);
        public override Color CooldownStartColor => new Color(216, 60, 90);
        public override Color CooldownEndColor => new Color(251, 106, 150);

        public override LegacySoundStyle EndSound => SoundLoader.GetLegacySoundSlot(GetInstance<CalamityMod>(), "Sounds/Custom/AbilitySounds/BloodflareRangerRecharge");
    }
}
