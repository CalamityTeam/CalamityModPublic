using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class TemporalSadness : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Temporal Sadness");
            Description.SetDefault("You are crying");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().tSad < npc.buffTime[buffIndex])
                npc.Calamity().tSad = npc.buffTime[buffIndex];
            if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().tSad;
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
