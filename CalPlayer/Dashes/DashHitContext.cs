using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public struct DashHitContext
    {
        public int BaseDamage;

        public float BaseKnockback;

        public DamageClass damageClass;

        public int PlayerImmunityFrames;

        public int HitDirection;
    }
}
