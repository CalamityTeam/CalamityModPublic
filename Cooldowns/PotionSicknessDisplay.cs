using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class PotionSicknessDisplay : Cooldown
    {
        public override bool ShouldDisplay => CalamityConfig.Instance.VanillaCooldownDisplay && AfflictedPlayer.potionDelay > 0;
        public override string DisplayName => "Healing Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PotionSickness";
        public override Color OutlineColor => new Color(255, 142, 165);
        public override Color CooldownStartColor => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), Completion);


        public PotionSicknessDisplay(int duration, Player player) : base(duration, player)
        {
        }
    }
}