using Microsoft.Xna.Framework;

namespace CalamityMod.Cooldowns
{
    public class PotionSickness : CooldownHandler
    {
        public static new string ID => "PotionSickness";
        public override bool ShouldDisplay => CalamityConfig.Instance.VanillaCooldownDisplay && instance.player.potionDelay > 0;
        public override string DisplayName => "Healing Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/PotionSickness";
        public override Color OutlineColor => new Color(255, 142, 165);
        public override Color CooldownStartColor => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), instance.Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), instance.Completion);
    }
}