using Terraria;

namespace CalamityMod.Balancing
{
    public interface IBalancingRule
    {
        bool AppliesTo(NPC npc, NPCHitContext hitContext);

        void ApplyBalancingChange(NPC npc, ref int damage);
    }
}
