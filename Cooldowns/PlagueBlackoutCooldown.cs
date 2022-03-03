using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace CalamityMod.Cooldowns
{
    public class PlagueBlackoutCooldown : Cooldown
    {
        public override string SyncID => "PlagueBlackout";
        public override bool ShouldDisplay => TimeLeft <= 1500;
        public override string DisplayName => "Plague Blackout Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PlagueBlackout";
        public override Color OutlineColor => new Color(174, 237, 122);
        public override Color CooldownStartColor => Color.DarkSlateGray;
        public override Color CooldownEndColor => Color.DarkSlateGray;
        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/PlagueReaperRecharge");

        public PlagueBlackoutCooldown(int duration, Player player) : base(duration, player)
        {
            Duration = 1500;
        }

        public override void OnCompleted()
        {
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(AfflictedPlayer.position, AfflictedPlayer.width, AfflictedPlayer.height, 89, 0, 0, 100, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 6.6f;
            }
        }
    }
}