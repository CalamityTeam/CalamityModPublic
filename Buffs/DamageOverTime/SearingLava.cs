using System;
using CalamityMod.DataStructures;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    [LegacyName("CragsLava")]
    public class SearingLava : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 40,
            HeatDebuffScaling = 2,
            NPCLifeRegenMethod = CragsLavaScaling
        };

        public static void CragsLavaScaling(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            //Applies the double Heat scaling but completely ignores NPC weakness/resistance to heat.

            //We need to apply slimed as that isn't included in HeatDebuffMultiplier due to needing to dynamically update if slimed is applied
            StatModifier multiplier = npc.Calamity().HeatDebuffMultiplier;
            if (npc.drippingSlime || npc.drippingSparkleSlime)
                multiplier += 1f;

            int dotValue = (int)DebuffData.ApplyScalingToStatModifer(multiplier, debuffData.HeatDebuffScaling).ApplyTo(debuffData.EnemyLostRegen);
            npc.Calamity().ApplyDPSDebuff(dotValue, (int)Math.Max(dotValue * debuffData.MultiplierDamageTickSize, debuffData.MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().searingLava = true;
        }
    }
}
