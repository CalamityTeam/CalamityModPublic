using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace CalamityMod.Cooldowns
{
    public class PlagueBlackout : CooldownHandler
    {
        public static string ID => "PlagueBlackout";
        public PlagueBlackout(CooldownInstance? c) : base(c) => instance.duration = 1500;

        public override bool ShouldDisplay => instance.timeLeft <= 1500;
        public override string DisplayName => "Plague Blackout Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/PlagueBlackout";
        public override Color OutlineColor => new Color(174, 237, 122);
        public override Color CooldownStartColor => Color.DarkSlateGray;
        public override Color CooldownEndColor => Color.DarkSlateGray;
        public override LegacySoundStyle EndSound => instance.player.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/PlagueReaperRecharge");

        public override void OnCompleted()
        {
            Vector2 pos = instance.player.position;
            int w = instance.player.width;
            int h = instance.player.height;
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(pos, w, h, 89, 0, 0, 100, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 6.6f;
            }
        }
    }
}