using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Balancing
{
    public interface IBalancingRule
    {
        bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile);

        void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers);
    }

    public class ClassResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public DamageClass ApplicableClass;
        public ClassResistBalancingRule(float damageMultiplier, DamageClass dc)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableClass = dc;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            // E.g. so that a melee resist also applies to MeleeNoSpeed, TrueMelee, and TrueMeleeNoSpeed.
            return modifiers.DamageType.CountsAsClass(ApplicableClass);
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

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile) => Requirement(npc);

        // This "balancing" rule doesn't actually perform any changes. It simply serves as a means of enforcing NPC-specific requirements, and should be used only as a filter.
        // As such, this method is empty.
        public void ApplyBalancingChange(NPC npc, ref NPC.HitModifiers modifiers) { }
    }

    public class PierceResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public PierceResistBalancingRule(float damageMultiplier) => DamageMultiplier = damageMultiplier;

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && (projectile.maxPenetrate > 1 || projectile.maxPenetrate == -1);
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

    public class ProjectileResistBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public int[] ApplicableProjectileTypes;
        public ProjectileResistBalancingRule(float damageMultiplier, params int[] projTypes)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableProjectileTypes = projTypes;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && ApplicableProjectileTypes.Contains(projectile.type);
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

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && Requirement(projectile);
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

    // TODO -- this rule type should be deleted once stealth is finished as its own class
    public class StealthStrikeBalancingRule : IBalancingRule
    {
        public float DamageMultiplier;
        public int[] ApplicableProjectileTypes;
        public StealthStrikeBalancingRule(float damageMultiplier, params int[] projTypes)
        {
            DamageMultiplier = damageMultiplier;
            ApplicableProjectileTypes = projTypes;
        }

        public bool AppliesTo(NPC npc, NPC.HitModifiers modifiers, Projectile? projectile)
        {
            return projectile is not null && (modifiers.DamageType == StealthDamageClass.Instance || projectile.Calamity().stealthStrike);
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
