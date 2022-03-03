using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Cooldowns
{
    public class InkBombCooldown : Cooldown
    {
        public override string SyncID => "InkBomb";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Ink Bomb Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/InkBomb";
        public override Color OutlineColor => new Color(205, 182, 137);
        public override Color CooldownStartColor => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);
        public override Color CooldownEndColor => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);


        public InkBombCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}