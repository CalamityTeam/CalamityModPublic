using CalamityMod.DataStructures;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class SagePoison : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 50,
            SicknessDebuffScaling = 1,
            NPCLifeRegenMethod = SagePoisonPower
        };

        public static float ViridVanguardPoisonMultiplier => 2;
        public static void SagePoisonPower(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            int viralCount = 0;
            int viridCount = 0;
            foreach (var item in Main.ActiveProjectiles)
            {
                if (item.type == ModContent.ProjectileType<SageSpirit>())
                    viralCount++;
                if (item.type == ModContent.ProjectileType<ViridVanguardBlade>())
                    viridCount++;
            }
            if ((viralCount + viridCount) <= 0)
                return;

            //Reduce power of weakness/resistances but leave sickness debuff boosters at full power
            //We first need to apply Irradiated as that isn't included in SicknessDebuffMultiplier due to needing to dynamically update if Irradiated is applied
            StatModifier multiplier = npc.Calamity().SicknessDebuffMultiplier;
            if (npc.Calamity().irradiated)
                multiplier += npc.Calamity().scionsCurioEffected ? 1.75f : 1f;

            bool wormBoss = CalamityNPCTypeSets.DesertScourge.Contains(npc.type) || CalamityNPCTypeSets.EaterOfWorlds.Contains(npc.type) || CalamityNPCTypeSets.Perforators.Contains(npc.type) ||
                CalamityNPCTypeSets.AquaticScourge.Contains(npc.type) || CalamityNPCTypeSets.AstrumDeus.Contains(npc.type) || CalamityNPCTypeSets.StormWeaver.Contains(npc.type);
            float NewWeaknessEffectiveness = 1.25f; //1.25x DPS instead of 2x - 1/4th the effectiveness
            float NewWeaknessEffectivenessWorm = 1.125f; //1.125x DPS instead of 1.5x - 1/4th the effectiveness
            float NewResistanceEffectiveness = 0.875f; // 0.875x instead of 0.5x - 1/4th the effectiveness

            if (npc.Calamity().VulnerableToSickness.HasValue)
                if (npc.Calamity().VulnerableToSickness.Value)
                    multiplier *= (wormBoss ? NewWeaknessEffectivenessWorm : NewWeaknessEffectiveness);
                else
                    multiplier *= NewResistanceEffectiveness;

            //Apply the DOT
            float SagePoisonRegen = (viralCount + ViridVanguardPoisonMultiplier * viridCount) * debuffData.EnemyLostRegen;
            int totalDoT = (int)multiplier.ApplyTo(SagePoisonRegen);
            npc.Calamity().ApplyDPSDebuff(totalDoT, totalDoT/(2*(viridCount+viralCount)), ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().sagePoison = true;
        }
    }
}
