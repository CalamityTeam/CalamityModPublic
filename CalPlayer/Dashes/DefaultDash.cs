using CalamityMod.Enums;
using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.CalPlayer.Dashes
{
    public class DefaultDash : PlayerDashEffect
    {
        public static new string ID => "Default Dash";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 1f;

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            hitContext.Damage = 0;
            hitContext.PlayerImmunityFrames = 0;
        }
    }
}
