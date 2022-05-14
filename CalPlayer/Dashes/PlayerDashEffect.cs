using Terraria;
using Terraria.DataStructures;

namespace CalamityMod.CalPlayer.Dashes
{
    public abstract class PlayerDashEffect
    {
        // For the sake of easy access without the need for stored instances, this property is defined as static instead of abstract.
        // Derived classes should use the new keyword on this member to define their own version of it.
        public static string ID { get; }

        public abstract bool IsOmnidirectional { get; }

        public abstract float CalculateDashSpeed(Player player);

        public virtual void OnDashEffects(Player player) { }

        public virtual void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor) { }

        public virtual void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext) { }
    }
}
