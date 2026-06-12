using System;
using CalamityMod.DataStructures;
using CalamityMod.Items.Accessories;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.StatBuffs
{
    public class SmashedEvil : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 40,
            SicknessDebuffScaling = 1,
            HeatDebuffScaling = 1,
            ColdDebuffScaling = 1,
            ElectricDebuffScaling = 1,
            WaterDebuffScaling = 1,
            NPCLifeRegenMethod = SmashedEvilNPCRegen

        };

        public static void SmashedEvilNPCRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();
            float totalDPS = debuffData.EnemyLostRegen;

            var heat = cnpc.ActiveHeatDebuffMultiplier;
            var cold = cnpc.ActiveColdDebuffMultiplier;
            var sick = cnpc.ActiveSicknessDebuffMultiplier;
            var elec = cnpc.ActiveElectricDebuffMultiplier;
            var water = cnpc.ActiveWaterDebuffMultiplier;
            StatModifier totalScaling =
                 DebuffData.ForceModifierPositiveWithScaling(cnpc.ActiveHeatDebuffMultiplier, debuffData.HeatDebuffScaling)
                    .CombineWith(DebuffData.ForceModifierPositiveWithScaling(cnpc.ActiveColdDebuffMultiplier, debuffData.ColdDebuffScaling)
                    .CombineWith(DebuffData.ForceModifierPositiveWithScaling(cnpc.ActiveSicknessDebuffMultiplier, debuffData.SicknessDebuffScaling)
                    .CombineWith(DebuffData.ForceModifierPositiveWithScaling(cnpc.ActiveWaterDebuffMultiplier, debuffData.WaterDebuffScaling)
                    .CombineWith(DebuffData.ForceModifierPositiveWithScaling(cnpc.ActiveElectricDebuffMultiplier, debuffData.ElectricDebuffScaling)
                 ))));
            totalDPS = totalScaling.ApplyTo(totalDPS);
            var totalDPSAdjusted = totalDPS - debuffData.EnemyVanillaRegenToCancelOut;
            npc.Calamity().ApplyDPSDebuff((int)(totalDPSAdjusted), (int)Math.Max(totalDPS * debuffData.MultiplierDamageTickSize, debuffData.MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegenTime += 2;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.moveSpeed += 0.2f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
