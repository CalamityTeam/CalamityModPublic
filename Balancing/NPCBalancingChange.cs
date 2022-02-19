namespace CalamityMod.Balancing
{
    public struct NPCBalancingChange
    {
        public int NPCType;
        public IBalancingRule[] BalancingRules;
        public NPCBalancingChange(int npcType, params IBalancingRule[] balancingRules)
        {
            NPCType = npcType;
            BalancingRules = balancingRules;
        }
    }
}
