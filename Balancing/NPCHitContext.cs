using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Balancing
{
    public struct NPCHitContext
    {
        public int Pierce;
        public int Damage;
        public int? ProjectileIndex;
        public int? ProjectileType => ProjectileIndex.HasValue ? Main.projectile[ProjectileIndex.Value].type : (int?)null;
        public bool IsStealthStrike;
        public ClassType? Class;
        public DamageSourceType DamageSource;

        public static NPCHitContext ConstructFromProjectile(Projectile proj)
        {
            ClassType? classType = null;
            if (proj.active)
            {
                if (proj.CountsAsClass<MeleeDamageClass>())
                    classType = ClassType.Melee;
                if (proj.CountsAsClass<RangedDamageClass>())
                    classType = ClassType.Ranged;
                if (proj.CountsAsClass<MagicDamageClass>())
                    classType = ClassType.Magic;
                if (proj.minion)
                    classType = ClassType.Summon;
                if (proj.CountsAsClass<RogueDamageClass>())
                    classType = ClassType.Rogue;
            }

            return new NPCHitContext()
            {
                Pierce = proj.penetrate,
                Damage = proj.damage,
                ProjectileIndex = proj.whoAmI,
                Class = classType,
                IsStealthStrike = proj.active && proj.Calamity().stealthStrike,
                DamageSource = DamageSourceType.FriendlyProjectile
            };
        }
    }
}
