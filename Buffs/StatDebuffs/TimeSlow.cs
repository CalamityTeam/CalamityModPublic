using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class TimeSlow : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Time Distortion");
            Description.SetDefault("Time is slowed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().timeSlow < npc.buffTime[buffIndex])
                npc.Calamity().timeSlow = npc.buffTime[buffIndex];
            if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().timeSlow;
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        /*public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().timeSlow = true;
        }*/
    }
}
