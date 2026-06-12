using CalamityMod.Enums;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public abstract class PlayerDashEffect : ModType
    {
        // For the sake of easy access without the need for stored instances, this property is defined as static instead of abstract.
        // Derived classes should use the new keyword on this member to define their own version of it.
        public static string ID { get; }
        public int dashTime { get; set; }
        public virtual int dashStartup => 0;
        public virtual string DashID => field ??= FullName;

        public int DashTimeAdjustedForStartup => dashTime - dashStartup;

        public abstract DashCollisionType CollisionType { get; }

        public abstract bool IsOmnidirectional { get; }

        public virtual int ForceEndAt { get; } = -1;

        public abstract float CalculateDashSpeed(Player player);
        /// <summary>
        /// Effects that happen once when the main dash starts
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnDashEffects(Player player) { }
        /// <summary>
        /// Effects that happen once when the dash startup starts
        /// This does not run on dashes without set startup
        /// </summary>
        /// <param name="player"></param>

        public virtual void OnDashStartupEffects(Player player) { }
        /// <summary>
        /// Effects that run during the dash startup time
        /// This does not run on dashes without set startup
        /// </summary>
        /// <param name="player"></param>
        public virtual void DashStartupEffects(Player player) { }

        public virtual void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor) { }

        public virtual void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext) { }

        protected sealed override void Register()
        {
            ModTypeLookup<PlayerDashEffect>.Register(this);
            PlayerDashManager.DashIdentificationTable[DashID] = this;
        }
    }
}
