using Terraria;

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
                if (proj.melee)
                    classType = ClassType.Melee;
                if (proj.ranged)
                    classType = ClassType.Ranged;
                if (proj.magic)
                    classType = ClassType.Magic;
                if (proj.minion)
                    classType = ClassType.Summon;
                if (proj.Calamity().rogue)
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
