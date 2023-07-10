using System.Linq;
using Terraria;

namespace CalamityMod.Balancing
{
    public interface IBalancingRule
    {
        bool AppliesTo(NPC npc, NPCHitContext hitContext);

        void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers);
    }

    public class ClassResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public ClassType ApplicableClass;
        public ClassResistBalancingRule(float damageMultiplier, ClassType classType)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableClass = classType;
        }

        public bool AppliesTo(NPC npc, NPCHitContext hitContext)
        {
            return hitContext.Class == ApplicableClass;
        }

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Changed from FinalDamage to SourceDamage to ensure that resists will also apply to on-hit effects.
            // See this message in the Calamity Dev Discord for full rationale.
            // https://discord.com/channels/458428222061936650/459003706575421440/1128002932000960592
            //
            // There is one limitation to this approach. Flat bonus damage, aka whip tags, won't be reduced.
            // In the case that summons or whips need to be resisted, the best move is to consider them entirely separately.
            modifiers.SourceDamage *= DamageMultiplier;
        }
    }

    public class NPCSpecificRequirementBalancingRule : IBalancingRule
    {
        public NPCApplicationRequirement Requirement;
        public delegate bool NPCApplicationRequirement(NPC npc);
        public NPCSpecificRequirementBalancingRule(NPCApplicationRequirement npcApplicationRequirement)
        {
            Requirement = npcApplicationRequirement;
        }

        public bool AppliesTo(NPC npc, NPCHitContext hitContext) => Requirement(npc);

        // This "balancing" rule doesn't actually perform any changes. It simply serves as a means of enforcing NPC-specific requirements, and should be used only as a filter.
        // As such, this method is empty.
        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) { }
    }

    public class PierceResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public PierceResistBalancingRule(float damageMultiplier) => DamageMultiplier = damageMultiplier;

        public bool AppliesTo(NPC npc, NPCHitContext hitContext) => hitContext.Pierce > 1 || hitContext.Pierce == -1;

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Changed from FinalDamage to SourceDamage to ensure that resists will also apply to on-hit effects.
            // See this message in the Calamity Dev Discord for full rationale.
            // https://discord.com/channels/458428222061936650/459003706575421440/1128002932000960592
            //
            // There is one limitation to this approach. Flat bonus damage, aka whip tags, won't be reduced.
            // In the case that summons or whips need to be resisted, the best move is to consider them entirely separately.
            modifiers.SourceDamage *= DamageMultiplier;
        }
    }

    public class ProjectileResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public int[] ApplicableProjectileTypes;
        public ProjectileResistBalancingRule(float damageMultiplier, params int[] projTypes)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableProjectileTypes = projTypes;
        }

        public bool AppliesTo(NPC npc, NPCHitContext hitContext)
        {
            if (hitContext.DamageSource != DamageSourceType.FriendlyProjectile)
                return false;
            if (!ApplicableProjectileTypes.Contains(hitContext.ProjectileType ?? -1))
                return false;

            return true;
        }

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Changed from FinalDamage to SourceDamage to ensure that resists will also apply to on-hit effects.
            // See this message in the Calamity Dev Discord for full rationale.
            // https://discord.com/channels/458428222061936650/459003706575421440/1128002932000960592
            //
            // There is one limitation to this approach. Flat bonus damage, aka whip tags, won't be reduced.
            // In the case that summons or whips need to be resisted, the best move is to consider them entirely separately.
            modifiers.SourceDamage *= DamageMultiplier;
        }
    }

    public class ProjectileSpecificRequirementBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public ProjectileApplicationRequirement Requirement;
        public delegate bool ProjectileApplicationRequirement(Projectile proj);
        public ProjectileSpecificRequirementBalancingRule(float damageMultiplier, ProjectileApplicationRequirement projApplicationRequirement)
        {
            DamageMultiplier = damageMultiplier;
            Requirement = projApplicationRequirement;
        }

        public bool AppliesTo(NPC npc, NPCHitContext hitContext)
        {
            if (hitContext.DamageSource != DamageSourceType.FriendlyProjectile)
                return false;

            return Requirement(Main.projectile[hitContext.ProjectileIndex.Value]);
        }

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Changed from FinalDamage to SourceDamage to ensure that resists will also apply to on-hit effects.
            // See this message in the Calamity Dev Discord for full rationale.
            // https://discord.com/channels/458428222061936650/459003706575421440/1128002932000960592
            //
            // There is one limitation to this approach. Flat bonus damage, aka whip tags, won't be reduced.
            // In the case that summons or whips need to be resisted, the best move is to consider them entirely separately.
            modifiers.SourceDamage *= DamageMultiplier;
        }
    }

    public class StealthStrikeBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public int[] ApplicableProjectileTypes;
        public StealthStrikeBalancingRule(float damageMultiplier, params int[] projTypes)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableProjectileTypes = projTypes;
        }

        public bool AppliesTo(NPC npc, NPCHitContext hitContext)
        {
            if (hitContext.DamageSource != DamageSourceType.FriendlyProjectile)
                return false;
            if (!ApplicableProjectileTypes.Contains(hitContext.ProjectileType ?? -1))
                return false;

            return hitContext.IsStealthStrike;
        }

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Changed from FinalDamage to SourceDamage to ensure that resists will also apply to on-hit effects.
            // See this message in the Calamity Dev Discord for full rationale.
            // https://discord.com/channels/458428222061936650/459003706575421440/1128002932000960592
            //
            // There is one limitation to this approach. Flat bonus damage, aka whip tags, won't be reduced.
            // In the case that summons or whips need to be resisted, the best move is to consider them entirely separately.
            modifiers.SourceDamage *= DamageMultiplier;
        }
    }

    public class TrueMeleeResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public TrueMeleeResistBalancingRule(float damageMultiplier)
        {
            DamageMultiplier = damageMultiplier;
        }

        public bool AppliesTo(NPC npc, NPCHitContext hitContext)
        {
            if (hitContext.DamageSource == DamageSourceType.FriendlyProjectile)
                return Main.projectile[hitContext.ProjectileIndex.Value].IsTrueMelee();

            return hitContext.DamageSource == DamageSourceType.TrueMeleeSwing;
        }

        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers)
        {
            // Changed from FinalDamage to SourceDamage to ensure that resists will also apply to on-hit effects.
            // See this message in the Calamity Dev Discord for full rationale.
            // https://discord.com/channels/458428222061936650/459003706575421440/1128002932000960592
            //
            // There is one limitation to this approach. Flat bonus damage, aka whip tags, won't be reduced.
            // In the case that summons or whips need to be resisted, the best move is to consider them entirely separately.
            modifiers.SourceDamage *= DamageMultiplier;
        }
    }
}
