using CalamityMod.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class BanishingFire : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 4000, //This is the minimum DoT. Banishing Fire scales with max HP
            HeatDebuffScaling = 1, //Unused in the method, but kept so other things can know this is a heat debuff
            NPCLifeRegenMethod = BanishingFireNPCLifeRegen
        };
        public static void BanishingFireNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            int baseBanishingFireDoTValue = (int)npc.Calamity().ActiveHeatDebuffMultiplier.ApplyTo((npc.lifeMax >= 1000000 ? npc.lifeMax / 500 : debuffData.EnemyLostRegen));
            npc.Calamity().ApplyDPSDebuff(baseBanishingFireDoTValue, baseBanishingFireDoTValue / 5, ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().banishingFire = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().banishingFire = true;
        }
    }
}
